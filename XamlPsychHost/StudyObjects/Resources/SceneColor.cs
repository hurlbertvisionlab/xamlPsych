using System;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class SceneColor : DependencyObject, ILogInfo
    {
        public static readonly DependencyProperty LuminaireProperty = DependencyProperty.Register(nameof(Luminaire), typeof(Luminaire), typeof(SceneColor));

        public Luminaire Luminaire
        {
            get { return (Luminaire)GetValue(LuminaireProperty); }
            set { SetValue(LuminaireProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(LuminaireColor), typeof(SceneColor));

        public LuminaireColor Color
        {
            get { return (LuminaireColor)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public string ToLogString(StudyContext context)
        {
            return context.ToLogString(Luminaire, Color);
        }
    }

}
