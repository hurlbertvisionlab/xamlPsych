using System.Collections.Generic;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ResourceSource : StudyDataSource
    {
        public TokenStringCollection Keys { get; set; } = new TokenStringCollection();

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            foreach (string key in Keys)
            {
                object resource = context.Study.FindResource(key);
                context.LogRegisterName(resource, key);

                if (resource is StudyDataSource source)
                    foreach (object item in source.GenerateItems(context))
                        yield return item;
                else
                    yield return resource;
            }
        }

        public override int? GetItemsCount(StudyContext context)
        {
            if (Keys == null || Keys.Count < 1)
                return 0;

            int itemsCount = 0;

            foreach (string key in Keys)
                if (context.Study.FindResource(key) is StudyDataSource source)
                {
                    if (source.GetItemsCount(context) is int sourceCount)
                        itemsCount += sourceCount;
                    else
                        return null;
                }
                else
                    itemsCount++;

            return itemsCount;
        }
    }
}
