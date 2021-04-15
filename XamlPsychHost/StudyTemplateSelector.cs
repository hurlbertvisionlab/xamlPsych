//using System.Windows;
//using System.Windows.Controls;

//namespace HurlbertVisionLab.XamlPsychHost
//{
//    public class StudyTemplateSelector : DataTemplateSelector
//    {
//        public override DataTemplate SelectTemplate(object item, DependencyObject container)
//        {
//            if (container is FrameworkElement { DataContext: StudyContext { CurrentStep: { Step: StudyUIStep step } } })
//            {
//                if (Application.Current.Resources.FindName(step.Layout + "Layout") is DataTemplate template)
//                    return template;
//            }

//            return base.SelectTemplate(item, container);
//        }
//    }
//}
