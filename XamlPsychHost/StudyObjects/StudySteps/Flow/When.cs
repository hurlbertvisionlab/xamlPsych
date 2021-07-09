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
    public class When : StudyStepCollection
    {
        public static readonly DependencyProperty StoreKeyProperty = DependencyProperty.Register(nameof(StoreKey), typeof(string), typeof(When));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(When));

        public string StoreKey
        {
            get { return (string)GetValue(StoreKeyProperty); }
            set { SetValue(StoreKeyProperty, value); }
        }

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Store.TryGetValue(StoreKey, out object value);

            if (object.Equals(value, Value))
                return base.Execute(cancellationToken);

            return Task.CompletedTask;
        }
    }
}
