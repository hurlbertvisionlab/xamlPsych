using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(Steps))]
    public class BreakOpportunity : StudyIterator
    {
        public static readonly DependencyProperty BreakGroupProperty = DependencyProperty.Register(nameof(BreakGroup), typeof(string), typeof(BreakOpportunity));
        public static readonly DependencyProperty AfterProperty = DependencyProperty.Register(nameof(After), typeof(TimeSpan), typeof(BreakOpportunity));
        public static readonly DependencyProperty EveryHitCountProperty = DependencyProperty.Register(nameof(EveryHitCount), typeof(int), typeof(BreakOpportunity));

        public int EveryHitCount
        {
            get { return (int)GetValue(EveryHitCountProperty); }
            set { SetValue(EveryHitCountProperty, value); }
        }

        public string BreakGroup
        {
            get { return (string)GetValue(BreakGroupProperty); }
            set { SetValue(BreakGroupProperty, value); }
        }

        public TimeSpan After
        {
            get { return (TimeSpan)GetValue(AfterProperty); }
            set { SetValue(AfterProperty, value); }
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            DateTimeOffset studyStarted = StudyContext.Started.GetValueOrDefault();
            bool breakOpportunity = true;

            if (After >= TimeSpan.Zero)
            {
                DateTimeOffset lastBreakEnded = studyStarted;
                if (StudyContext.Store.TryGetValue("LastBreakEnded:" + BreakGroup, out object value) && value is DateTimeOffset timestamp)
                    lastBreakEnded = timestamp;

                TimeSpan sinceLastBreak = DateTimeOffset.Now - lastBreakEnded;
                breakOpportunity = sinceLastBreak >= After;
                
                StudyContext.Log(this, this, nameof(After), breakOpportunity, sinceLastBreak);
            }

            if (EveryHitCount > 0)
            {
                int hitCount = 0;
                if (StudyContext.Store.TryGetValue("BreakHitCount:" + BreakGroup, out object value) && value is int count)
                    hitCount = count;

                StudyContext.Store["BreakHitCount:" + BreakGroup] = ++hitCount;

                int modulo = hitCount % EveryHitCount;
                breakOpportunity = modulo == 0;

                StudyContext.Log(this, this, nameof(EveryHitCount), breakOpportunity, hitCount);
            }

            if (breakOpportunity)
            {
                DateTimeOffset breakStarted = DateTimeOffset.Now;
                StudyContext.Log(this, this, "BreakStart", BreakGroup, breakStarted - studyStarted);
                await StudyContext.Execute(Steps, ItemContext, cancellationToken);
                
                DateTimeOffset breakEnded = DateTimeOffset.Now;
                StudyContext.Log(this, this, "BreakEnd", BreakGroup, breakEnded - breakStarted);
                
                StudyContext.Store["LastBreakEnded:" + BreakGroup] = breakEnded;
                StudyContext.Store["LastBreakEnded:"] = breakEnded; // any break
            }
        }
    }

    public class ResetBreakOpportunity : StudyStep
    {
        public static readonly DependencyProperty BreakGroupProperty = DependencyProperty.Register(nameof(BreakGroup), typeof(string), typeof(ResetBreakOpportunity));

        public string BreakGroup
        {
            get { return (string)GetValue(BreakGroupProperty); }
            set { SetValue(BreakGroupProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Store["LastBreakEnded:" + BreakGroup] = DateTimeOffset.Now;
            StudyContext.Store["BreakHitCount:" + BreakGroup] = 0;
            return Task.CompletedTask;
        }
    }
}
