using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(ValidationRules))]
    public class AskForTextInput : StudyUIStep
    {
        public static readonly DependencyProperty InputProperty = DependencyProperty.Register(nameof(Input), typeof(string), typeof(AskForTextInput), new PropertyMetadata(null, OnInputChanged));

        private static void OnInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StudyStep step)
            {
                step.StudyContext.Log(step, step, "Input", e.NewValue);
                step.Result = e.NewValue;
            }
        }

        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(AskForTextInput), new PropertyMetadata(null));
        public static readonly DependencyProperty ContinueTextProperty = DependencyProperty.Register(nameof(ContinueText), typeof(string), typeof(AskForTextInput), new PropertyMetadata("Continue"));

        public string ContinueText
        {
            get { return (string)GetValue(ContinueTextProperty); }
            set { SetValue(ContinueTextProperty, value); }
        }

        public string Prompt
        {
            get { return (string)GetValue(PromptProperty); }
            set { SetValue(PromptProperty, value); }
        }

        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        static AskForTextInput()
        {
            AdvanceWhenDoneProperty.OverrideMetadata(typeof(AskForTextInput), new PropertyMetadata(false));
        }

        public Collection<ValidationRule> ValidationRules { get; } = new Collection<ValidationRule>();
        public System.Collections.ICollection ValidationErrors { get; } = new object[0];

        protected override Task Execute(CancellationToken cancellationToken)
        {
            StudyContext.Log(this, this, "Prompt", Prompt);
            StudyContext.Screen.Show(this);
            return Task.CompletedTask;
        }

        public override void OnStudyInput(object sender, StudyInputEventArgs args)
        {
            if (args.Input == "Confirm")
                Advance(args.Input);

            args.Handled = true; // do not log key presses
        }

    }
}
