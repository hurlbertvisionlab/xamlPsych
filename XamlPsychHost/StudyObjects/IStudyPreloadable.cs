using System;
using System.Threading;
using System.Threading.Tasks;

namespace HurlbertVisionLab.XamlPsychHost
{
    public interface IStudyPreloadable
    {
        Task DoPreload(StudyContext context, IProgress<string> progress, CancellationToken cancellationToken);
        bool Preload { get; set; }
    }
}
