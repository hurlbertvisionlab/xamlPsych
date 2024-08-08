using System;
using System.Threading;
using System.Threading.Tasks;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class TimeStamp : StudyStep
    {
        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Store["Timer:" + ID] = DateTimeOffset.Now;
            
            return Task.CompletedTask;
        }
    }
}
