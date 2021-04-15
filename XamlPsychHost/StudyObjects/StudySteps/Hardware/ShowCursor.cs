using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ShowCursor : StudyStep
    {
        protected override Task Execute(CancellationToken cancellationToken)
        {
            if (StudyContext.Screen is Window window)
                window.ClearValue(Window.CursorProperty);            

            return Task.CompletedTask;
        }
    }
}
