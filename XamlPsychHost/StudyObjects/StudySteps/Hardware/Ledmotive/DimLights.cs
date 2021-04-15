using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class DimLights : StudyStep
    {
        public static readonly DependencyProperty LuminaireProperty = DependencyProperty.Register(nameof(Luminaire), typeof(Luminaire), typeof(DimLights));
        public static readonly DependencyProperty LevelProperty = DependencyProperty.Register(nameof(Level), typeof(double), typeof(DimLights), new PropertyMetadata(100.0));

        public double Level
        {
            get { return (double)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public Luminaire Luminaire
        {
            get { return (Luminaire)GetValue(LuminaireProperty); }
            set { SetValue(LuminaireProperty, value); }
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            if (Luminaire != null)
            {
                StudyContext.Log(this, this, "DimLights", Level);
                await Luminaire.Dim(Level);
            }
        }
    }

}
