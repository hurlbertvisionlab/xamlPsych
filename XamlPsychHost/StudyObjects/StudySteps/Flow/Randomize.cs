using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Randomize : StudyStepCollection
    {
        protected override async Task Execute(CancellationToken cancellationToken)
        {
            List<StudyStep> steps = this.ToList();
            
            List<int> indices = new List<int>(Count);
            for (int i = 0; i < Count; i++)
                indices.Add(i);


            while (steps.Count > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    StudyContext.Log(this, this, "Cancelled");
                    break;
                }

                int index = StudyContext.Random.Next(0, steps.Count);
                StudyStep step = steps[index];
                StudyContext.Log(this, this, "NextStep", indices[index], step.GetType().Name, step.ID);
                await StudyContext.Execute(steps[index], ItemContext, cancellationToken);
                indices.RemoveAt(index);
                steps.RemoveAt(index);
            }
        }
    }
}
