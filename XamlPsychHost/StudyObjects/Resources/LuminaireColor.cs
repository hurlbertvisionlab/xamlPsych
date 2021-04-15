using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class LuminaireColor : StudyObject, ILogInfo
    {
        public DoubleCollection Amplitudes { get; set; }

        public string ToLogString(StudyContext context)
        {
            return Amplitudes.ToString();
        }
    }
}
