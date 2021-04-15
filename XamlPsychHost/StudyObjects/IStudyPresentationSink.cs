using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public interface IStudyPresentationSink
    {
        void Show(object element);
    }
}
