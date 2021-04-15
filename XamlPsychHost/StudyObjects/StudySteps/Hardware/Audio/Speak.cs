using System;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Speak : StudyStep
    {
        public static readonly DependencyProperty VoiceProperty = DependencyProperty.Register(nameof(Voice), typeof(string), typeof(Speak));
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(Speak));
        public static readonly DependencyProperty RateProperty = DependencyProperty.Register(nameof(Rate), typeof(int), typeof(Speak));
        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(nameof(Volume), typeof(int), typeof(Speak), new PropertyMetadata(100));

        public int Volume
        {
            get { return (int)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }
        public int Rate
        {
            get { return (int)GetValue(RateProperty); }
            set { SetValue(RateProperty, value); }
        }
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public string Voice
        {
            get { return (string)GetValue(VoiceProperty); }
            set { SetValue(VoiceProperty, value); }
        }

        private readonly SpeechSynthesizer _synthesizer;
        private StudyContext _studyContext;

        public Speak()
        {
            _synthesizer = new SpeechSynthesizer();
            _synthesizer.StateChanged += OnStateChanged;
            _synthesizer.SpeakCompleted += OnSpeakCompleted;
        }

        protected virtual void OnSpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            Advance("SpeakCompleted");
        }

        protected virtual void OnStateChanged(object sender, StateChangedEventArgs e)
        {
            _studyContext?.Log(this, _synthesizer, "StateChanged", e.State);
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            _studyContext = StudyContext;
            _studyContext.Log(this, this, "Text", Text);

            _synthesizer.SetOutputToDefaultAudioDevice();
            _synthesizer.Rate = Rate;

            if (Voice != null)
                _synthesizer.SelectVoice(Voice);

            _synthesizer.SpeakAsync(Text);
            return Task.CompletedTask;
        }
    }
}
