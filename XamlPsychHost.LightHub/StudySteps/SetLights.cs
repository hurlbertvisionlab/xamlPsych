using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty("Scene")]
    public class SetLights : StudyStep
    {
        public static readonly DependencyProperty SceneProperty = DependencyProperty.Register(nameof(Scene), typeof(LuminaireScene), typeof(SetLights));
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(LuminaireColor), typeof(SetLights));
        public static readonly DependencyProperty LuminaireProperty = DependencyProperty.Register(nameof(Luminaire), typeof(Luminaire), typeof(SetLights));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(nameof(Scale), typeof(double), typeof(SetLights), new PropertyMetadata(1.0));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public Luminaire Luminaire
        {
            get { return (Luminaire)GetValue(LuminaireProperty); }
            set { SetValue(LuminaireProperty, value); }
        }

        public LuminaireScene Scene
        {
            get { return (LuminaireScene)GetValue(SceneProperty); }
            set { SetValue(SceneProperty, value); }
        }

        public LuminaireColor Color
        {
            get { return (LuminaireColor)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            if (Luminaire != null && Color != null)
            {
                StudyContext.Log(this, this, "SetLights", Scale, "Color", Color);
                await Luminaire.Show(Color, Scale);
            }

            if (Scene != null)
            {
                StudyContext.Log(this, this, "SetLights", Scale, "Scene", Scene);
                await Scene.Show(Scale);
            }
        }
    }
}
