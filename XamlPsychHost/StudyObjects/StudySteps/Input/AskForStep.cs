using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class AskForStep : StudyUIStep
    {
        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(AskForStep), new PropertyMetadata(null));
        public static readonly DependencyProperty ContinueTextProperty = DependencyProperty.Register(nameof(ContinueText), typeof(string), typeof(AskForStep), new PropertyMetadata("Continue"));

        public string ContinueText
        {
            get { return (string)GetValue(ContinueTextProperty); }
            set { SetValue(ContinueTextProperty, value); }
        }

        public string Prompt
        {
            get { return (string)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        protected static void LogAndStoreAsResult(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StudyStep step)
            {
                step.StudyContext.Log(step, step, "Input", e.NewValue);
                step.Result = e.NewValue;
            }
        }

        static AskForStep()
        {
            AdvanceWhenDoneProperty.OverrideMetadata(typeof(AskForStep), new PropertyMetadata(false));
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Log(this, this, "Prompt", Prompt);
            StudyContext.Screen.Show(this);
            return Task.CompletedTask;
        }

        public override void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            if (args.Input == "Confirm")
                Advance(args.Input);

            args.Handled = true; // do not log key presses
        }
    }
}
