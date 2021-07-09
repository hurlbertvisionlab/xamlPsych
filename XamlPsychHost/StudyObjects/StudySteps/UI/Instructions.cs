using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Instructions : StudyUIStep
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Instructions), new PropertyMetadata(null, null, CoerceText));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Log(this, this, "Text", Text);
            StudyContext.Screen.Show(this);
            return Task.CompletedTask;
        }
    }
}
