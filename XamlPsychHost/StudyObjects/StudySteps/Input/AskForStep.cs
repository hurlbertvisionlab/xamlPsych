using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class AskForStep : StudyUIStep
    {
        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(nameof(Prompt), typeof(object), typeof(AskForStep), new PropertyMetadata(null, null, CoerceText));
        public static readonly DependencyProperty ContinueTextProperty = DependencyProperty.Register(nameof(ContinueText), typeof(string), typeof(AskForStep), new PropertyMetadata("Continue", null, CoerceText));

        public string ContinueText
        {
            get { return (string)GetValue(ContinueTextProperty); }
            set { SetValue(ContinueTextProperty, value); }
        }

        public object Prompt
        {
            get { return GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        private string GetTextForLog(object o)
        {
            if (o == null)
                return null;

            if (o is string s) return s;
            if (o is ILogInfo info) return info.ToLogString(StudyContext);
            if (o is TextBlock tb) return tb.Text;
            if (o is TextBox tbx) return tbx.Text;
            if (o is RichTextBox rtb) return rtb.Document.ToString();
            if (o is Label lb) return GetTextForLog(lb.Content);
            if (o is Image { Source: BitmapImage i }) return i.UriSource?.ToString();
            if (o is DependencyObject d)
            {
                foreach (var child in LogicalTreeHelper.GetChildren(d))
                    if (GetTextForLog(child) is string childText)
                        return childText;
            }

            return o.ToString();
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
            StudyContext.Log(this, this, "Prompt", GetTextForLog(Prompt));
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
