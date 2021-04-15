using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace HurlbertVisionLab.XamlPsychHost
{
    [TypeConverter(typeof(TokenStringSetConverter))]
    public class TokenStringSet : HashSet<string>
    {
        public TokenStringSet() : base(StringComparer.OrdinalIgnoreCase) { }
        public TokenStringSet(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase) { }
        public TokenStringSet(IEnumerable<string> collection) : base(collection, StringComparer.OrdinalIgnoreCase) { }
    }

    public class TokenStringSetConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
                return new TokenStringSet(s.Split(App.SplitChars));

            return base.ConvertFrom(context, culture, value);
        }
    }
}
