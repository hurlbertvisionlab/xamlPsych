using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace HurlbertVisionLab.XamlPsychHost
{
    [TypeConverter(typeof(TokenStringCollectionConverter))]
    public class TokenStringCollection : List<string>
    {
        public TokenStringCollection() { }
        public TokenStringCollection(int capacity) : base(capacity) { }
        public TokenStringCollection(IEnumerable<string> collection) : base(collection) { }
    }

    public class TokenStringCollectionConverter : TypeConverter
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
                return new TokenStringCollection(s.Split(App.SplitChars));

            return base.ConvertFrom(context, culture, value);
        }
    }
}
