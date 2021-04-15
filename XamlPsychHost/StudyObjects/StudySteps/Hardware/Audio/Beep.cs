using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Beep : StudyStep
    {
        public static readonly DependencyProperty FrequencyProperty = DependencyProperty.Register(nameof(Frequency), typeof(int), typeof(Beep), new UIPropertyMetadata(1000));
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(Beep), new UIPropertyMetadata(TimeSpan.FromMilliseconds(1000)));

        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public int Frequency
        {
            get { return (int)GetValue(FrequencyProperty); }
            set { SetValue(FrequencyProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Log(this, this, "Beep", Frequency, Duration);            
            Console.Beep(Frequency, (int)Duration.TotalMilliseconds);
            return Task.CompletedTask;
        }
    }
}
