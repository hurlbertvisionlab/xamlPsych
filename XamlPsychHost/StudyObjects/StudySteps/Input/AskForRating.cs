using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class AskForRating : AskForStep
    {
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(double), typeof(AskForRating), new PropertyMetadata(1.0));
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(AskForRating), new PropertyMetadata(5.0));
        public static readonly DependencyProperty StepProperty = DependencyProperty.Register(nameof(Step), typeof(double), typeof(AskForRating), new PropertyMetadata(1.0));
        public static readonly DependencyProperty MinimumTextProperty = DependencyProperty.Register(nameof(MinimumText), typeof(string), typeof(AskForRating));
        public static readonly DependencyProperty MaximumTextProperty = DependencyProperty.Register(nameof(MaximumText), typeof(string), typeof(AskForRating));
        public static readonly DependencyProperty StartingValueProperty = DependencyProperty.Register(nameof(StartingValue), typeof(DefaultDouble), typeof(AskForRating), new PropertyMetadata(DefaultDouble.Auto));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(double), typeof(AskForRating), new PropertyMetadata(3.0, LogAndStoreAsResult));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public DefaultDouble StartingValue
        {
            get { return (DefaultDouble)GetValue(StartingValueProperty); }
            set { SetValue(StartingValueProperty, value); }
        }

        public string MaximumText
        {
            get { return (string)GetValue(MaximumTextProperty); }
            set { SetValue(MaximumTextProperty, value); }
        }

        public string MinimumText
        {
            get { return (string)GetValue(MinimumTextProperty); }
            set { SetValue(MinimumTextProperty, value); }
        }

        public double Step
        {
            get { return (double)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        protected override void PrepareForNewExecution()
        {
            switch (StartingValue.ValueType)
            {
                case DefaultValueType.Auto:
                    break;

                case DefaultValueType.Absolute:
                    Value = StartingValue.Value;
                    break;

                case DefaultValueType.Default:
                    Value = (Maximum - Minimum) / 2;
                    break;

                case DefaultValueType.Minimum:
                case DefaultValueType.First:
                    Value = Minimum;
                    break;

                case DefaultValueType.Maximum:
                case DefaultValueType.Last:
                    Value = Maximum;
                    break;

                case DefaultValueType.Random:
                    if (Step == 0)
                    {
                        Value = Minimum + (Maximum - Minimum) * StudyContext.Random.NextDouble();
                    }
                    else
                    {
                        int steps = checked((int)((Maximum - Minimum) / Step));
                        int index = StudyContext.Random.Next(-1, steps) + 1;
                        Value = Minimum + index * Step;
                    }
                    break;
            }

            base.PrepareForNewExecution();
        }
    }
}
