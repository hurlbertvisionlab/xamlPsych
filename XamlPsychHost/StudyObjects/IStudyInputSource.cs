using System;
using System.Windows;
using System.Windows.Markup;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "HurlbertVisionLab.XamlPsychHost")]

namespace HurlbertVisionLab.XamlPsychHost
{
    public interface IStudyInputSource : IInputElement
    {
        event StudyInputRoutedEventHandler StudyInput;
        void ReportStudyInput(IInputProvider source, string toInput);
    }
}
