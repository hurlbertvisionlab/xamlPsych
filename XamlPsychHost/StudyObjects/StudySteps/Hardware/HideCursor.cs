using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class HideCursor : StudyStep
    {
        protected override Task Execute(CancellationToken cancellationToken)
        {
            if (StudyContext.Screen is Window window)
                window.Cursor = Cursors.None;

            return Task.CompletedTask;
        }
    }
}
