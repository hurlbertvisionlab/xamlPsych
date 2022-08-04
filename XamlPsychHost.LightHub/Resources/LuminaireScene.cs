using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty("SceneColors")]
    public class LuminaireScene : DependencyObject, ILogInfo
    {
        public Collection<SceneColor> SceneColors { get; set; } = new Collection<SceneColor>();

        public async Task Show(double scale = 1.0)
        {
            foreach (SceneColor sc in SceneColors)
                await sc.Luminaire.Show(sc.Color, sc.Scale * scale);
        }

        public string ToLogString(StudyContext context)
        {
            return context.ToLogString(SceneColors);
        }
    }

}
