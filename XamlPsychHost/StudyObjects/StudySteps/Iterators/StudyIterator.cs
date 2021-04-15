namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyIterator : StudyStep
    {
        public StudyStepCollection Steps { get; set; } = new StudyStepCollection();
    }

}
