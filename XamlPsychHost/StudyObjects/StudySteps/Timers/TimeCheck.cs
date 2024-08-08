using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class TimeCheck : StudyStep
    {
        public static readonly DependencyProperty StampIDProperty = DependencyProperty.Register(nameof(StampID), typeof(string), typeof(TimeCheck));

        public string StampID
        {
            get { return (string)GetValue(StampIDProperty); }
            set { SetValue(StampIDProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            if (StudyContext.Store.TryGetValue("Timer:" + StampID, out var value) && value is DateTimeOffset start)
                Result = DateTimeOffset.Now - start;
            else
                Result = "N/A";

            return Task.CompletedTask;
        }
    }
}
