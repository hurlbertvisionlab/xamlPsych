using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class VibrateGamepad : StudyStep
    {
        public static readonly DependencyProperty RightMotorSpeedProperty = DependencyProperty.Register(nameof(RightMotorSpeed), typeof(ushort), typeof(VibrateGamepad), new PropertyMetadata(OnRightMotorSpeedChanged));
        public static readonly DependencyProperty LeftMotorSpeedProperty = DependencyProperty.Register(nameof(LeftMotorSpeed), typeof(ushort), typeof(VibrateGamepad), new PropertyMetadata(OnLeftMotorSpeedChanged));

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(VibrateGamepad));
        public TimeSpan Duration
        {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        public ushort RightMotorSpeed
        {
            get { return (ushort)GetValue(RightMotorSpeedProperty); }
            set { SetValue(RightMotorSpeedProperty, value); }
        }

        private static void OnRightMotorSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VibrateGamepad)d)._vibration.RightMotorSpeed = (ushort)e.NewValue;
        }

        public ushort LeftMotorSpeed
        {
            get { return (ushort)GetValue(LeftMotorSpeedProperty); }
            set { SetValue(LeftMotorSpeedProperty, value); }
        }

        private static void OnLeftMotorSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VibrateGamepad)d)._vibration.LeftMotorSpeed = (ushort)e.NewValue;
        }

        private XInput.Vibration _vibration = new XInput.Vibration();

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Log(this, this, "SetState", _vibration.LeftMotorSpeed, _vibration.RightMotorSpeed);
            XInput.SetState(0, _vibration);
            if (Duration.Ticks > 0)
            {
                await Task.Delay(Duration).ConfigureAwait(false);
                StudyContext.Log(this, this, "SetState", 0, 0);
                XInput.SetState(0, default);
            }
        }
    }
}
