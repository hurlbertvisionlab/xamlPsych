using System;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyUIStep : StudyStep
    {
        protected static object CoerceText(DependencyObject d, object baseValue)
        {
            if (baseValue is string s)
                baseValue = s.Replace("  ", Environment.NewLine);

            return baseValue;
        }
    }
}
