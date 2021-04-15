using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(Mappings))]
    public class KeyboardInputProvider : EnumInputProvider<Key, MapKeyboard>
    {
        protected override void Bind()
        {
            InputSource.PreviewKeyDown += OnPreviewKeyDown;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            ReportInput(e.Key);
        }

        public override void Unbind()
        {
            InputSource.PreviewKeyDown -= OnPreviewKeyDown;
        }

        protected override MapKeyboard CreateMap(MapInput map)
        {
            return new MapKeyboard { Key = MapKeyboard.GetKey(map), ToInput = map.ToInput };
        }
    }
}
