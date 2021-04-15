using System;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class MapInput : DependencyObject
    {
        public static readonly DependencyProperty ToInputProperty = DependencyProperty.Register(nameof(ToInput), typeof(string), typeof(MapInput));

        public string ToInput
        {
            get { return (string)GetValue(ToInputProperty); }
            set { SetValue(ToInputProperty, value); }
        }
    }

    public interface IEnumMap<TKey>
    {
        TKey Key { get; set; }
        string ToInput { get; set; }
    }
}
