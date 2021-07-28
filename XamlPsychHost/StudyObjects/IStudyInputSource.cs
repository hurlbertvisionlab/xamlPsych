using System;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public interface IStudyInputSource : IInputElement
    {
        event StudyInputRoutedEventHandler StudyInput;
        void ReportStudyInput(IInputProvider source, string toInput);
    }
}
