using System;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyUIStep : StudyStep
    {
        public static readonly DependencyProperty RemainingToAdvanceDisplayProperty = DependencyProperty.Register(nameof(RemainingToAdvanceDisplay), typeof(RemainingDisplay), typeof(StudyUIStep), new PropertyMetadata(RemainingDisplay.Text));

        public RemainingDisplay RemainingToAdvanceDisplay
        {
            get { return (RemainingDisplay)GetValue(RemainingToAdvanceDisplayProperty); }
            set { SetValue(RemainingToAdvanceDisplayProperty, value); }
        }

        protected static object CoerceText(DependencyObject d, object baseValue)
        {
            if (baseValue is string s)
                baseValue = s.Replace("  ", Environment.NewLine);

            return baseValue;
        }
    }
}
