using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public enum Order
    {
        Default,
        Randomized,
        Ascending,
        Descending
    }

    [ContentProperty(nameof(Steps))]
    public class ForEach : StudyIterator
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(StudyDataSource), typeof(ForEach));
        public static readonly DependencyProperty ItemsOrderProperty = DependencyProperty.Register(nameof(ItemsOrder), typeof(Order), typeof(ForEach));
        public static readonly DependencyProperty SkipProperty = DependencyProperty.Register(nameof(Skip), typeof(int), typeof(ForEach), new PropertyMetadata(0));
        public static readonly DependencyProperty TakeProperty = DependencyProperty.Register(nameof(Take), typeof(int), typeof(ForEach), new PropertyMetadata(int.MaxValue));

        public int Take
        {
            get { return (int)GetValue(TakeProperty); }
            set { SetValue(TakeProperty, value); }
        }

        public int Skip
        {
            get { return (int)GetValue(SkipProperty); }
            set { SetValue(SkipProperty, value); }
        }

        public Order ItemsOrder
        {
            get { return (Order)GetValue(ItemsOrderProperty); }
            set { SetValue(ItemsOrderProperty, value); }
        }

        public StudyDataSource ItemsSource
        {
            get { return (StudyDataSource)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            if (Steps == null || Steps.Count < 1)
                return Task.CompletedTask;

            Steps.ID ??= ID;

            StudyContext.Log(this, this, "Reset", Skip, Take, ItemsOrder);
            IEnumerable<object> source = ItemsSource.GenerateItems(StudyContext).Skip(Skip).Take(Take);

            switch (ItemsOrder)
            {
                case Order.Default:
                    return Execute(source, cancellationToken);

                case Order.Ascending:
                    return Execute(source.OrderBy(item => item), cancellationToken);

                case Order.Descending:
                    return Execute(source.OrderByDescending(item => item), cancellationToken);

                case Order.Randomized:
                    return ExecuteRandom(source.ToList(), cancellationToken);

                default:
                    throw new NotImplementedException();
            }
        }

        private async Task Execute(IEnumerable items, CancellationToken cancellationToken)
        {
            foreach (object item in items)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                StudyContext.Log(this, this, "NextItem");
                await StudyContext.Execute(Steps, item, cancellationToken);
            }
        }

        private async Task ExecuteRandom(IList items, CancellationToken cancellationToken)
        {
            List<int> indices = new List<int>(items.Count);
            for (int i = 0; i < items.Count; i++)
                indices.Add(i);

            while (items.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StudyContext.Log(this, this, "Cancelled");
                    break;
                }

                int index = StudyContext.Random.Next(0, items.Count);
                StudyContext.Log(this, this, "NextItem", index, indices[index]);
                await StudyContext.Execute(Steps, items[index], cancellationToken);
                items.RemoveAt(index);
                indices.RemoveAt(index);
            }
        }
    }

}
