namespace HurlbertVisionLab.XamlPsychHost
{
    public interface IInputProvider
    {
        void BindTo(IStudyInputSource element);
        void Unbind();
        void Map(MapInput item);
    }
}
