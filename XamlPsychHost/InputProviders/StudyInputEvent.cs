using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class StudyInputEventArgs : RoutedEventArgs
    {
        public string Input { get; }

        public StudyInputEventArgs(object source, string input) : base(Study.StudyInputEvent, source)
        {
            Input = input;
        }
    }

    public delegate void StudyInputRoutedEventHandler(object sender, StudyInputEventArgs args);
}
