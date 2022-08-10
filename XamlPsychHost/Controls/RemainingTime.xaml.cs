using System.Windows.Controls;

namespace HurlbertVisionLab.XamlPsychHost
{
    public enum RemainingDisplay
    {
        None,
        Text,
        Progress,
        ProgressIncreasing
    }

    public partial class RemainingTime : UserControl
    {
        public RemainingTime()
        {
            InitializeComponent();
        }
    }
}
