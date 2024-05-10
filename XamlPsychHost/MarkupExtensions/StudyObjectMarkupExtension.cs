using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyObjectMarkupExtension : MarkupExtension
    {
        public string Path { get; set;  }
        protected string Prefix { get; }

        public StudyObjectMarkupExtension(DependencyProperty property, string path)
        {
            Prefix = property.Name;
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return CreateBinding(serviceProvider).ProvideValue(serviceProvider);
        }

        internal Binding CreateBinding(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            RelativeSource source;
            if (target.TargetObject is StudyObject)
                source = new RelativeSource(RelativeSourceMode.Self);
            else
                source = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(StudyStep), 1);
            
            string path = Prefix;
            if (!string.IsNullOrEmpty(Path) && !Path.StartsWith("#"))
            {
                if (Path.StartsWith("["))
                    path += Path;
                else
                    path += "." + Path;
            }

            return new Binding(path) { RelativeSource = source };
        }
    }

    public class ItemContextExtension : StudyObjectMarkupExtension
    {
        public ItemContextExtension() : this(null) { }
        public ItemContextExtension(string path) : base(StudyObject.ItemContextProperty, path) { }
    }
    public class StepContextExtension : StudyObjectMarkupExtension
    {
        public StepContextExtension() : this(null) { }
        public StepContextExtension(string path) : base(StudyObject.StepContextProperty, path) { }
    }
    public class StudyContextExtension : StudyObjectMarkupExtension
    {
        public StudyContextExtension() : this(null) { }
        public StudyContextExtension(string path) : base(StudyObject.StudyContextProperty, path) { }
    }
    public class StoreValueExtension : StudyObjectMarkupExtension
    {
        public StoreValueExtension() : this(null) { }
        public StoreValueExtension(string name) : base(StudyObject.StudyContextProperty, string.IsNullOrEmpty(name) ? null : "Store[" + name + "]") { }
    }
}
