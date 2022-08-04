using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(ContentTemplate))]
    public class ShowStimuli : StudyUIStep
    {
        public static readonly DependencyProperty InstructionsAboveProperty = DependencyProperty.Register(nameof(InstructionsAbove), typeof(string), typeof(ShowStimuli), new PropertyMetadata(null, null, CoerceText));
        public static readonly DependencyProperty InstructionsBelowProperty = DependencyProperty.Register(nameof(InstructionsBelow), typeof(string), typeof(ShowStimuli), new PropertyMetadata(null, null, CoerceText));

        public string InstructionsBelow
        {
            get { return (string)GetValue(InstructionsBelowProperty); }
            set { SetValue(InstructionsBelowProperty, value); }
        }

        public string InstructionsAbove
        {
            get { return (string)GetValue(InstructionsAboveProperty); }
            set { SetValue(InstructionsAboveProperty, value); }
        }

        static ShowStimuli()
        {
            AdvanceWhenDoneProperty.OverrideMetadata(typeof(ShowStimuli), new PropertyMetadata(false));
        }

        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register(nameof(ContentTemplate), typeof(DataTemplate), typeof(ShowStimuli));
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ShowStimuli));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Screen.Show(this); // TODO: this may take time to render, the timeout timer should be started or reset after rendering is done
            return Task.CompletedTask;
        }
    }
}
