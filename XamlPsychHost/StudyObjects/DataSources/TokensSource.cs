using System.Collections.Generic;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(Tokens))]
    public class TokensSource : StudyDataSource
    {
        public TokenStringCollection Tokens { get; set; } = new TokenStringCollection();

        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            return Tokens;
        }

        public override int? GetItemsCount(StudyContext context)
        {
            return Tokens?.Count ?? 0;
        }
    }
}
