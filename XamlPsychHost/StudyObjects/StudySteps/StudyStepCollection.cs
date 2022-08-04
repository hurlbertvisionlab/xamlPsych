using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class StudyStepCollection : StudyStep, ICollection<StudyStep>
    {
        private readonly ICollection<StudyStep> _steps = new List<StudyStep>();

        public int Count => _steps.Count;
        public bool IsReadOnly => _steps.IsReadOnly;
        public void Clear() => _steps.Clear();
        public void Add(StudyStep item) => _steps.Add(item);
        public bool Remove(StudyStep item) => _steps.Remove(item);
        public bool Contains(StudyStep item) => _steps.Contains(item);
        public void CopyTo(StudyStep[] array, int arrayIndex) => _steps.CopyTo(array, arrayIndex);
        public IEnumerator<StudyStep> GetEnumerator() => _steps.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_steps).GetEnumerator();

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            foreach (StudyStep step in _steps)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StudyContext.Log(this, this, "Cancelled");
                    break;
                }

                await StudyContext.Execute(step, ItemContext, cancellationToken);
            }
        }
    }
}
