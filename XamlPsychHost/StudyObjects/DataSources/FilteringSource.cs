using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class FilteringSource : ResourceSource
    {
        public static readonly DependencyProperty FlattenProperty = DependencyProperty.Register(nameof(Flatten), typeof(bool), typeof(FilteringSource));
        public static readonly DependencyProperty TakeProperty = DependencyProperty.Register(nameof(Take), typeof(int), typeof(FilteringSource));
        public static readonly DependencyProperty SkipProperty = DependencyProperty.Register(nameof(Skip), typeof(int), typeof(FilteringSource));
        public Predicate<object> Filter { get; set; }

        public int Skip
        {
            get { return (int)GetValue(SkipProperty); }
            set { SetValue(SkipProperty, value); }
        }

        public int Take
        {
            get { return (int)GetValue(TakeProperty); }
            set { SetValue(TakeProperty, value); }
        }

        public bool Flatten
        {
            get { return (bool)GetValue(FlattenProperty); }
            set { SetValue(FlattenProperty, value); }
        }

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            if (Keys == null)
                yield break;

            int skipped = 0;
            int taken = 0;
            Predicate<object> filter = Filter;

            // the take, skip and filter are meant to operate on items before flattening

            foreach (string key in Keys)
            {
                object resource = context.Study.FindResource(key);
                context.LogRegisterName(resource, key);

                IEnumerable items;

                if (resource is StudyDataSource source)
                    items = source.GenerateItems(context);
                else
                    items = new object[] { resource };

                foreach (var item in items)
                {
                    if (filter != null && !filter(item))
                        continue;

                    if (skipped < Skip)
                    {
                        skipped++;
                        continue;
                    }

                    if (FlattenItem(item, context) is IEnumerable multiple) // TODO: consider recursive (or user-defined level) of flattening
                        foreach (var subitem in multiple)
                            yield return subitem;
                    else
                        yield return item;

                    taken++;

                    if (Take > 0 && taken >= Take)
                        yield break;
                }
            }
        }

        private IEnumerable FlattenItem(object item, StudyContext context)
        {
            if (Flatten)
            {
                if (item is StudyDataSource source)
                    return source.GenerateItems(context);

                if (item is CombinedItem combined)
                    return combined.Values;

                else if (item is PickedItem picked)
                    return picked;

                else if (item is ICollection collection)
                    return collection;
            }

            return null;
        }

        public override int? GetItemsCount(StudyContext context)
        {
            if (Keys == null || Keys.Count < 1)
                return 0;

            int count = 0; // the count is after flattening, so there isn't much optimisation we can do
            int take = Take;

            // fast path
            if (Flatten == false && Filter == null)
            {
                if (base.GetItemsCount(context) is int allCount)
                {
                    allCount -= Math.Max(0, Skip);
                    if (Take > 0) return Math.Min(allCount, take);
                    return Math.Max(0, allCount);
                }
            }

            return GenerateItems(context).Count();
        }
    }
}
