using System;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyUIStep : StudyStep
    {
        //public static readonly DependencyProperty DataContextProperty = FrameworkElement.DataContextProperty.AddOwner(typeof(StudyUIStep));

        //public object DataContext
        //{
        //    get { return (object)GetValue(DataContextProperty); }
        //    set { SetValue(DataContextProperty, value); }
        //}
        
        protected static object CoerceText(DependencyObject d, object baseValue)
        {
            if (baseValue is string s)
                baseValue = s.Replace("  ", Environment.NewLine);

            return baseValue;
        }
    }
}
