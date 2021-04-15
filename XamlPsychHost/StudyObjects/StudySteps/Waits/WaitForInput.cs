using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class WaitForInput : StudyStep
    {
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register(nameof(Input), typeof(string), typeof(WaitForInput));

        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        private TaskCompletionSource<string> _taskCompletion;

        protected override Task Execute(CancellationToken cancellationToken)
        {
            _taskCompletion = new TaskCompletionSource<string>();
            return _taskCompletion.Task;
        }

        public override void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            base.OnStudyInput(sender, args);

            if (Input == null || Input == args.Input)
                _taskCompletion.SetResult(args.Input);
        }
    }
}
