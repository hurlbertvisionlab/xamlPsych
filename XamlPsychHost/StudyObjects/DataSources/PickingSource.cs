using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public enum PickingMode
    {
        Permutation,
        Combination,
        CombinationRandomized,
    }

    public class PickingSource : StudyDataSource
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(StudyDataSource), typeof(PickingSource));
        public static readonly DependencyProperty PickProperty = DependencyProperty.Register(nameof(Pick), typeof(int), typeof(PickingSource), new PropertyMetadata(1));
        public static readonly DependencyProperty AllowRepeatsProperty = DependencyProperty.Register(nameof(AllowRepeats), typeof(bool), typeof(PickingSource));
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(PickingMode), typeof(PickingSource), new UIPropertyMetadata(PickingMode.Combination));
        

        public PickingMode Mode
        {
            get { return (PickingMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        public bool AllowRepeats
        {
            get { return (bool)GetValue(AllowRepeatsProperty); }
            set { SetValue(AllowRepeatsProperty, value); }
        }

        public int Pick
        {
            get { return (int)GetValue(PickProperty); }
            set { SetValue(PickProperty, value); }
        }

        public StudyDataSource Source
        {
            get { return (StudyDataSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public override int? GetItemsCount(StudyContext context)
        {
            if (Pick <= 1)
                return Source?.GetItemsCount(context);

            if (Source == null)
                return 0;

            if (Source.GetItemsCount(context) is not int sourceCount)
                return null;

            if (!AllowRepeats && Pick > sourceCount)
                throw new StudyException(context, $"Not enough items in the '{Source.GetType().Name}' data source to pick {Pick} items and repeats are not allowed.", null, this);

            switch (Mode)
            {
                case PickingMode.Permutation:
                    return AllowRepeats ? (int)Math.Pow(sourceCount, Pick) : Factorial(sourceCount, sourceCount - Pick);

                case PickingMode.Combination:
                case PickingMode.CombinationRandomized:
                    return (AllowRepeats ? Factorial(Pick + sourceCount - 1, sourceCount - 1) : Factorial(sourceCount, sourceCount - Pick)) / Factorial(Pick);

                default:
                    throw new NotImplementedException();
            }

            static int Factorial(int n, int to = 1) => n <= to ? 1 : n * Factorial(n - 1, to);
        }

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            if (Pick <= 1)
                return Source?.GenerateItems(context);

            if (Source == null)
                return new object[0];

            return GenerateItemsInternal(context);
        }

        private IEnumerable<object> GenerateItemsInternal(StudyContext context)
        {
            Debug.Assert(Source != null);

            object[] items = Source.GenerateItems(context).ToArray();
            int[] indices = new int[Pick];

            if (!AllowRepeats)
            {
                if (Pick > items.Length)
                    throw new StudyException(context, $"Not enough items in the '{Source.GetType().Name}' data source to pick {Pick} items and repeats are not allowed.", null, this);

                for (int i = 0; i < indices.Length; i++)
                    indices[i] = i;
            }

            while (true)
            {
                if (AllowRepeats || !HasRepeats(indices))
                {
                    object[] combination = new object[Pick];
                    for (int i = 0; i < indices.Length; i++)
                        combination[i] = items[indices[i]];

                    int[] indicesSnapshot = (int[])indices.Clone();
                    if (Mode == PickingMode.CombinationRandomized)
                        Shuffle(combination, indicesSnapshot, context);

                    yield return new PickedItem(combination, indicesSnapshot);
                }

                bool next = false;

                switch (Mode)
                {
                    case PickingMode.Permutation:
                        next = NextPermutationIndex(items.Length, indices);
                        break;

                    case PickingMode.Combination:
                    case PickingMode.CombinationRandomized:
                        next = NextCombinationIndex(items.Length, indices);
                        break;

                    default:
                        break;
                }

                if (!next)
                    yield break;
            }
        }

        private static void Shuffle(object[] combination, int[] indices, StudyContext context)
        {
            for (int i = combination.Length - 1; i >= 0; i--)
            {
                int j = context.Random.Next(0, i + 1); // exclusive bound
                Swap(combination, i, j);
                Swap(indices, i, j);
            }

            static void Swap<T>(T[] a, int i, int j) { T o = a[i]; a[i] = a[j]; a[j] = o; }
        }

        private static bool HasRepeats(int[] indices)
        {
            for (int i = 0; i < indices.Length - 1; i++)
                for (int j = i + 1; j < indices.Length; j++)
                    if (indices[i] == indices[j])
                        return true;

            return false;
        }

        private static bool NextPermutationIndex(int modulo, params int[] indices)
        {
            int position = indices.Length - 1;

            while (++indices[position] >= modulo)
            {
                indices[position] = 0;
                if (--position < 0)
                    return false;
            }

            return true;
        }

        private static bool NextCombinationIndex(int modulo, params int[] indices)
        {
            // we only allow growing (or equal) indices
            int position = indices.Length - 1;

            // PERF: when repeats are not allowed, the "zeroeing" of remaining indices should be incremental
            // as a consequence, the max of the leading digits is decreasing so that the remaining positions fit an incrementing "zeroing"

            while (++indices[position] >= modulo)
            {
                if (position == 0)
                    return false;

                for (int i = position; i < indices.Length; i++)
                    indices[i] = indices[position - 1] + 1;

                --position;
            }

            return true;
        }
    }
}
