using System;
using System.Collections.Generic;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class StudyStepContext
    {
        public StudyStepContext(StudyStep step, int number)
        {
            Number = number;
            Started = DateTimeOffset.Now;
            Step = step;
        }

        public int Number { get; }
        public StudyStep Step { get; }
        public DateTimeOffset Started { get; }
        public DateTimeOffset Ended { get; set; }
        
        public StudyContext StudyContext { get; set; }
        public object ItemContext { get; set; }
        public IDictionary<string, object> ItemContexts => StudyContext.ItemContexts;

        public string Name => Step?.GetType().Name;
    }
}
