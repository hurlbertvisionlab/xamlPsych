using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class UnsafeSingleConverter<T> : TypeConverter where T : struct
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
            {
                string[] tokens = s.Split(App.SplitChars);
                if (Marshal.SizeOf<T>() != tokens.Length * sizeof(float))
                    throw new ArgumentException(nameof(value));

                IntPtr pColor = Marshal.AllocHGlobal(tokens.Length * sizeof(float));
                try
                {
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        float parsed = float.Parse(tokens[i], culture);
                        unsafe { Marshal.WriteInt32(pColor, i * sizeof(float), *(int*)&parsed); }
                    }

                    T color = Marshal.PtrToStructure<T>(pColor);
                    return color;
                }
                finally
                {
                    Marshal.FreeHGlobal(pColor);
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
