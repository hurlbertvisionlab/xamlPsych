using System;

namespace HurlbertVisionLab.XamlPsychHost
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class StudyInputProviderAttribute : Attribute
    {
        public Type Type { get; set; }

        public StudyInputProviderAttribute(Type type) => Type = type;
    }
}
