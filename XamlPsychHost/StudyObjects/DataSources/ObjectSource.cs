using System.Collections.Generic;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(Items))]
    public class ObjectSource : StudyDataSource
    {
        public List<object> Items { get; set; }

        public ObjectSource()
        {
            Items = new List<object>();
        }
        public ObjectSource(params object[] items)
        {
            Items = new List<object>(items);
        }

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            return Items;
        }

        public override int? GetItemsCount(StudyContext context)
        {
            return Items?.Count ?? 0;
        }
    }
}
