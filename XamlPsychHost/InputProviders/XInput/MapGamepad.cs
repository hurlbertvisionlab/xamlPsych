using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    [StudyInputProvider(typeof(GamepadInputProvider))]    
    public class MapGamepad : MapInput, IEnumMap<XInput.VirtualKey>
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached(nameof(Key), typeof(XInput.VirtualKey), typeof(MapGamepad));

        public static XInput.VirtualKey GetKey(MapInput obj)
        {
            return (XInput.VirtualKey)obj.GetValue(KeyProperty);
        }
        public static void SetKey(MapInput obj, XInput.VirtualKey value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public XInput.VirtualKey Key
        {
            get { return (XInput.VirtualKey)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }
    }
}
