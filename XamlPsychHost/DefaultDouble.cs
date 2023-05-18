using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace HurlbertVisionLab.XamlPsychHost
{
    public enum DefaultValueType
    {
        Auto,
        Absolute,
        Default,
        Minimum,
        Maximum,
        First,
        Last,
        Random,
    }

    [TypeConverter(typeof(DefaultValueConverter))]
    public struct DefaultDouble : IEquatable<DefaultDouble>, IFormattable
    {
        double _value;
        DefaultValueType _valueType;

        public DefaultDouble(DefaultValueType valueType) : this(double.NaN, valueType) { }
        public DefaultDouble(double value) : this(value, DefaultValueType.Absolute) { }
        public DefaultDouble(double value, DefaultValueType valueType)
        {
            _value = value;
            _valueType = valueType;
        }

        public double Value
        {
            get
            {
                if (_valueType == DefaultValueType.Absolute)
                    return _value;

                return double.NaN;
            }
        }
        public DefaultValueType ValueType => _valueType;

        public static bool operator ==(DefaultDouble a, DefaultDouble b) => a.ValueType == b.ValueType && a.Value == b.Value;
        public static bool operator !=(DefaultDouble a, DefaultDouble b) => a.ValueType != b.ValueType || a.Value != b.Value;
        public override bool Equals(object obj) => obj is DefaultDouble def ? this == def : base.Equals(obj);
        public bool Equals(DefaultDouble other) => this == other;
        public override int GetHashCode() => (int)_value + (int)_valueType;

        public bool IsAbsolute => _valueType == DefaultValueType.Absolute;
        public bool IsAuto => _valueType == DefaultValueType.Auto;

        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }
        public string ToString(IFormatProvider formatProvider)
        {
            if (ValueType == DefaultValueType.Absolute)
                return Value.ToString(formatProvider);

            return ValueType.ToString();
        }

        string IFormattable.ToString(string format, IFormatProvider formatProvider) => ToString(formatProvider);

        public static readonly DefaultDouble Auto = new DefaultDouble(DefaultValueType.Auto);
    }

    public class DefaultValueConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            TypeCode tc = Type.GetTypeCode(sourceType);
            switch (tc)
            {
                case TypeCode.String:
                case TypeCode.Decimal:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object source)
        {
            if (source is string s)
            {
                if (double.TryParse(s, NumberStyles.AllowExponent | NumberStyles.Number, culture, out double value))
                    return new DefaultDouble(value);
             
                if (Enum.TryParse(s, out DefaultValueType valueType))
                    return new DefaultDouble(valueType);
            }
            else if (source is not null)
            {
                DefaultValueType type = DefaultValueType.Absolute;
                double value = Convert.ToDouble(source, culture);

                if (double.IsNaN(value))
                {
                    value = double.NaN;
                    type = DefaultValueType.Auto;
                }

                return new DefaultDouble(value, type);
            }

            return base.ConvertFrom(context, culture, source);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is DefaultDouble def)
            {
                if (destinationType == typeof(string))
                {
                    return def.ToString(culture);
                }

                if (destinationType == typeof(InstanceDescriptor))
                {
                    ConstructorInfo ci = typeof(DefaultDouble).GetConstructor(new Type[] { typeof(double), typeof(DefaultValueType) });
                    return new InstanceDescriptor(ci, new object[] { def.Value, def.ValueType });
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}