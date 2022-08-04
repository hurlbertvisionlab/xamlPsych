using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(Steps))]
    public class Repeat : StudyIterator
    {
        public static readonly DependencyProperty TimesProperty = DependencyProperty.Register(nameof(Times), typeof(uint), typeof(Repeat));

        public uint Times
        {
            get { return (uint)GetValue(TimesProperty); }
            set { SetValue(TimesProperty, value); }
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            if (Steps == null || Steps.Count < 1)
                return;

            Steps.ID ??= ID;
            StudyContext.Log(this, this, "Reset", Times);

            for (int i = 0; i < Times; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StudyContext.Log(this, this, "Cancelled");
                    break;
                }

                StudyContext.Log(this, this, "NextItem", i);
                await StudyContext.Execute(Steps, ItemContext, cancellationToken);
            }
        }
    }
}
