using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ImageItem : ILogInfo, IStructuralEquatable 
    {
        public ImageSource Image { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }

        public override string ToString() => Name ?? Source ?? base.ToString();
        public string ToLogString(StudyContext context) => ToString();

        public bool Equals(object obj, IEqualityComparer comparer)
        {
            if (obj is not ImageItem other)
                return false;

            if (!comparer.Equals(this.Source, other.Source))
                return false;

            if (!comparer.Equals(this.Name, other.Name))
                return false;

            return true;
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            int hashCode = -2113663506;
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(Source);
            hashCode = hashCode * -1521134295 + comparer.GetHashCode(Name);
            return hashCode;
        }
    }

    public class ImageItemConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ImageSource))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(ImageSource))
                return ((ImageItem)value).Image;

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
