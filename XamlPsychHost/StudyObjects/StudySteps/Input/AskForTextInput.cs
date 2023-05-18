using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(ValidationRules))]
    public class AskForTextInput : AskForStep
    {
        public static readonly DependencyProperty StartingInputProperty = DependencyProperty.Register(nameof(StartingInput), typeof(string), typeof(AskForTextInput));
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register(nameof(Input), typeof(string), typeof(AskForTextInput), new PropertyMetadata(null, LogAndStoreAsResult));

        public string StartingInput
        {
            get { return (string)GetValue(StartingInputProperty); }
            set { SetValue(StartingInputProperty, value); }
        }
        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public Collection<ValidationRule> ValidationRules { get; } = new Collection<ValidationRule>();
        public System.Collections.ICollection ValidationErrors { get; } = new object[0];

        protected override void PrepareForNewExecution()
        {
            Input = StartingInput;
            base.PrepareForNewExecution();
        }

    }
}
