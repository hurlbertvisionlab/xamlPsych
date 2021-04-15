using System.Windows;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    [StudyInputProvider(typeof(KeyboardInputProvider))]    
    public class MapKeyboard : MapInput, IEnumMap<Key>
    {
        public static readonly DependencyProperty KeyProperty = DependencyProperty.RegisterAttached(nameof(Key), typeof(Key), typeof(MapKeyboard));

        public static Key GetKey(MapInput obj)
        {
            return (Key)obj.GetValue(KeyProperty);
        }
        public static void SetKey(MapInput obj, Key value)
        {
            obj.SetValue(KeyProperty, value);
        }

        public Key Key
        {
            get { return (Key)GetValue(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }
    }
}
