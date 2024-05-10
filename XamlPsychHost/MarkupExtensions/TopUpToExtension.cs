using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class TopUpToExtension : MarkupExtension
    {
        private class TopUpConverter : IMultiValueConverter
        {
            private TopUpToExtension _parameters;
            private StudyContext _itemsContext;
            private IList _allItems;

            public TopUpConverter(TopUpToExtension forExtension)
            {
                _parameters = forExtension;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                StudyContext context = (StudyContext)values[0];
                object value = values[1];

                if (context == null || value == null)
                    return value;

                if (context == DependencyProperty.UnsetValue || value == DependencyProperty.UnsetValue)
                    return DependencyProperty.UnsetValue;

                if (_itemsContext != context)
                {
                    // TODO: maybe we could have GenerateRandomItems or an indexer
                    IEnumerable<object> allItems = _parameters.From.GenerateItems(context);
                    _allItems = (allItems as IList) ?? allItems.ToList();
                    
                    _itemsContext = context;
                }

                object[] result = new object[_parameters.Count];
                int[] indices = new int[_parameters.Count];
                
                List<int> unusedIndices = new List<int>(_allItems.Count);
                for (int i = 0; i < _allItems.Count; i++)
                    unusedIndices.Add(i);

                for (int i = 0; i < result.Length; i++)
                {
                    int randomIndex = context.Random.Next(unusedIndices.Count);
                    int index = unusedIndices[randomIndex];
                    unusedIndices.RemoveAt(randomIndex);
                    
                    indices[i] = index;
                    result[i] = _allItems[index];
                }

                bool valuePresent = false;
                for (int i = 0; i < result.Length; i++)
                    if (StructuralComparisons.StructuralEqualityComparer.Equals(result[i], value))
                    {
                        valuePresent = true;
                        break;
                    }

                if (!valuePresent)
                {
                    int index = context.Random.Next(result.Length);
                    result[index] = value;
                    indices[index] = _allItems.IndexOf(value);
                }

                context.Log(context.CurrentStep.Step, context.CurrentStep.Name, "TopUpIndexOrder", context.ToLogString(indices));
                return new PickedItem(result, indices);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }

        private int _count;
        public int Count
        {
            get { return _count; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _count = value;
            }
        }

        public StudyDataSource From { get; set; }
        public string Path { get; set; }

        public TopUpToExtension() { }
        public TopUpToExtension(int count)
        {
            _count = count;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget targetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            var contextBinding = new StudyContextExtension().CreateBinding(serviceProvider);
            var itemBinding = new ItemContextExtension(Path).CreateBinding(serviceProvider);

            MultiBinding binding = new MultiBinding();
            binding.Bindings.Add(contextBinding);
            binding.Bindings.Add(itemBinding);
            binding.Converter = new TopUpConverter(this);
            return binding.ProvideValue(serviceProvider);
        }
    }
}
