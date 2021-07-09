using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class AdvanceExtension : MarkupExtension
    {
        private string _reason;

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public AdvanceExtension() { }
        public AdvanceExtension(string reason)
        {
            _reason = reason;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (targetProvider != null)
            {
                MethodInfo advanceMethod = this.GetType().GetMethod(nameof(Advance), BindingFlags.NonPublic | BindingFlags.Instance);

                EventInfo targetEvent = targetProvider.TargetProperty as EventInfo; // standard event
                if (targetEvent != null)
                    return Delegate.CreateDelegate(targetEvent.EventHandlerType, this, advanceMethod);

                MethodInfo targetMethod = targetProvider.TargetProperty as MethodInfo; // routed event
                if (targetMethod != null)
                {
                    ParameterInfo[] parameters = targetMethod.GetParameters();
                    if (parameters.Length == 2 && typeof(Delegate).IsAssignableFrom(parameters[1].ParameterType))
                        return Delegate.CreateDelegate(parameters[1].ParameterType, this, advanceMethod);
                }
            }

            throw new InvalidOperationException();
        }

        private void Advance(object sender, EventArgs e)
        {
            if (sender is FrameworkElement target)
            {
                StudyStep targetStep = null;

                switch (target.DataContext)
                {
                    case StudyStep step:
                        targetStep = step;
                        break;

                    case StudyStepContext stepContext:
                        targetStep = stepContext.Step;
                        break;

                    case StudyContext studyContext:
                        targetStep = studyContext?.CurrentStep?.Step;
                        break;
                }

                if (targetStep != null)
                    targetStep.Advance(_reason);
            }
        }
    }
}
