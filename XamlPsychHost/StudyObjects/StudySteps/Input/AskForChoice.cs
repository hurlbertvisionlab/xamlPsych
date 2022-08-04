using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class ChoiceItem
    {
        public ChoiceItem() { DataIndex = -1; }
        public ChoiceItem(int dataIndex, object data)
        {
            DataIndex = dataIndex;
            Data = data;
        }

        public int DataIndex { get; set; }
        public object Data { get; set; }

        public override string ToString() => string.Join(",", DataIndex, Data); // TODO: StudyContext lookup LogName, both for Result and choice order
    }

    public class ChoiceItemCollection : ObservableCollection<ChoiceItem>
    {

    }

    public class LayoutConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string layout)
            {
                switch (layout)
                {
                    case "Horizontal":
                        {
                            var factory = new FrameworkElementFactory(typeof(StackPanel));
                            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                            return new ItemsPanelTemplate(factory);
                        }
                    case "Vertical":
                        {
                            var factory = new FrameworkElementFactory(typeof(StackPanel));
                            factory.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
                            return new ItemsPanelTemplate(factory);
                        }
                    case "UniformGrid":
                        return new ItemsPanelTemplate(new FrameworkElementFactory(typeof(UniformGrid)));
                }
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    [ContentProperty(nameof(Items))]
    public class AskForChoice : StudyUIStep
    {
        public static readonly DependencyProperty PromptProperty = DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(AskForChoice), new PropertyMetadata(null));
        public static readonly DependencyProperty ContinueTextProperty = DependencyProperty.Register(nameof(ContinueText), typeof(string), typeof(AskForChoice), new PropertyMetadata("Continue"));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(AskForChoice), new PropertyMetadata(OnItemsSourceChanged)); // TODO: consider taking ownership of StudyIterator
        public static readonly DependencyProperty ItemsOrderProperty = DependencyProperty.Register(nameof(ItemsOrder), typeof(Order), typeof(AskForChoice), new PropertyMetadata(OnItemsOrderChanged));
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(AskForChoice));
        public static readonly DependencyProperty LayoutProperty = DependencyProperty.Register(nameof(Layout), typeof(ItemsPanelTemplate), typeof(AskForChoice));

        [TypeConverter(typeof(LayoutConverter))]
        public ItemsPanelTemplate Layout
        {
            get { return (ItemsPanelTemplate)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public ChoiceItemCollection Items { get; } = new ChoiceItemCollection();
        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((AskForChoice)d).RegenerateItems();
        private static void OnItemsOrderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((AskForChoice)d).RegenerateItems();

        private void RegenerateItems()
        {
            Items.Clear();

            List<ChoiceItem> source = GetItemsSourceAsEnumerable()?.Select((data, dataIndex) => new ChoiceItem(dataIndex, data)).ToList();
            if (source == null)
                return;

            switch (ItemsOrder)
            {
                case Order.Default:
                    foreach (ChoiceItem item in source)
                        Items.Add(item);
                    break;

                case Order.Ascending:
                    foreach (ChoiceItem item in source.OrderBy(i => i.Data))
                        Items.Add(item);
                    break;

                case Order.Descending:
                    foreach (ChoiceItem item in source.OrderByDescending(i => i.Data))
                        Items.Add(item);
                    break;

                case Order.Randomized:
                    while (source.Count > 0)
                    {
                        int index = StudyContext.Random.Next(source.Count);
                        Items.Add(source[index]);
                        source.RemoveAt(index);
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
        private IEnumerable<object> GetItemsSourceAsEnumerable()
        {
            if (ItemsSource is null || StudyContext is null)
                return null;

            else if (ItemsSource is StudyDataSource studySource)
                return studySource.GenerateItems(StudyContext);

            else if (ItemsSource is IEnumerable enumerable)
                return enumerable.Cast<object>();

            else
                return new object[] { ItemsSource };
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public Order ItemsOrder
        {
            get { return (Order)GetValue(ItemsOrderProperty); }
            set { SetValue(ItemsOrderProperty, value); }
        }

        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

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

        static AskForChoice()
        {
            StudyContextProperty.OverrideMetadata(typeof(AskForChoice), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, OnItemsSourceChanged));
            AdvanceWhenDoneProperty.OverrideMetadata(typeof(AskForChoice), new PropertyMetadata(false));
        }

        protected override Task Execute(CancellationToken cancellationToken)
        {
            RegenerateItems();

            StudyContext.Log(this, this, "Prompt", Prompt);
            StudyContext.Log(this, this, "ChoiceOrder", Items?.Select(i => i.DataIndex).Cast<object>().ToArray());
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
