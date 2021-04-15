using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Media;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ImageItem : ILogInfo 
    {
        public ImageSource Image { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }

        public override string ToString() => Name ?? Source ?? base.ToString();
        public string ToLogString(StudyContext context) => ToString();
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
