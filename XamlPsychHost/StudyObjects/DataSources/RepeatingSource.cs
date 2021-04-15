using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class RepeatingSource : StudyDataSource
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(StudyDataSource), typeof(RepeatingSource));
        public static readonly DependencyProperty RepeatsProperty = DependencyProperty.Register(nameof(Repeats), typeof(Int32Collection), typeof(RepeatingSource));

        public Int32Collection Repeats
        {
            get { return (Int32Collection)GetValue(RepeatsProperty); }
            set { SetValue(RepeatsProperty, value); }
        }

        public StudyDataSource Source
        {
            get { return (StudyDataSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public RepeatingSource()
        {
            Repeats = new Int32Collection(1);
            Repeats.Add(1);
        }

        public override int? GetItemsCount(StudyContext context)
        {
            if (Source == null || Repeats == null)
                return 0;

            if (Source.GetItemsCount(context) is not int sourceCount)
                return null;

            int repeatsCount = Repeats.Count;
            if (repeatsCount == 1)
                return sourceCount * Repeats[0];

            int itemsCount = 0;
            for (int i = 0; i < sourceCount; i++)
                itemsCount += Repeats[i % repeatsCount];

            return itemsCount;
        }

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            if (Source == null || Repeats == null)
                yield break;

            int repeatsCount = Repeats.Count;
            //if (repeatsCount == 1)
            //    return Source.GenerateItems(context);

            int i = 0;
            foreach (object item in Source.GenerateItems(context))
            {
                int repeats = Repeats[i % repeatsCount];
                for (int r = 0; r < repeats; r++)
                    yield return item;

                i++;
            }
        }
    }
}
