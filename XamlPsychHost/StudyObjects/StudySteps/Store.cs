using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class Store : StudyStep
    {
        public static readonly DependencyProperty AsProperty = DependencyProperty.Register(nameof(As), typeof(string), typeof(Store));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(Store));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string As
        {
            get { return (string)GetValue(AsProperty); }
            set { SetValue(AsProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            if (As is string key)
                StudyContext.Store[key] = Value;

            return Task.CompletedTask;
        }
    }
}
