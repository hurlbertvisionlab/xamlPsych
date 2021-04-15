using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using UAM.Optics.ColorScience;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class IlluminantConverter : UnsafeSingleConverter<Color1931XYZ>
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string name)
            {
                FieldInfo field = typeof(Illuminant1931XYZ).GetField(name, BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                    return field.GetValue(null);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
