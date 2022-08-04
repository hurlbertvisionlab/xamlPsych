namespace HurlbertVisionLab.XamlPsychHost
{
    public interface IStudyDisposableResource
    {
        void Access(StudyContext context);
        void Dispose(StudyContext context);
    }
}
