using System;
using System.Linq;
using System.Text;
using System.Xml;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class StudyException : ApplicationException
    {
        public StudyStepContext StepContext { get; }
        public StudyContext StudyContext { get; }
        public IXmlLineInfo LineInfo { get; }

        public string LineInfoString => LineInfo?.HasLineInfo() == true ? $"line {LineInfo.LineNumber}, column {LineInfo.LinePosition} ({LineInfo.GetType().Name})" : "(not available)";
        public string StepStackString
        {
            get
            {
                StringBuilder s = new StringBuilder("Protocol");
                StudyStepContext[] array = StudyContext.GetCurrentStepStack();

                for (int i = array.Length - 1; i >= 0; i--)
                {
                    if (array[i].Step is StudyStepCollection)
                        continue;

                    s.Append(" / ");
                    s.Append(array[i].Number);
                    s.Append(":");
                    s.Append(array[i].Name);

                    if (array[i].Step.ID != null)
                        s.AppendFormat(" ({0})", array[i].Step.ID);
                }

                return s.ToString();
            }
        }

        public StudyException(StudyContext study, string message, Exception innerException, IXmlLineInfo lineInfo = null) : this(study.CurrentStep, message, innerException, lineInfo) { }
        public StudyException(StudyStepContext step, string message, Exception innerException, IXmlLineInfo lineInfo = null) : base(message, innerException)
        {
            StepContext = step;
            StudyContext = step.StudyContext;
            LineInfo = lineInfo ?? step.Step;
        }
    }
}
