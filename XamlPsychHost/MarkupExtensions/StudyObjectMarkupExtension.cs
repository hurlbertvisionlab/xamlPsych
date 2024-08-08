using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace HurlbertVisionLab.XamlPsychHost
{
    public abstract class StudyObjectMarkupExtension : MarkupExtension
    {
        private class Duplicate : StudyObjectMarkupExtension
        {
            public Duplicate(StudyObjectMarkupExtension clone) : base(clone.Prefix, clone.Path) { }
        }

        public string Path { get; set;  }
        protected string Prefix { get; }

        public StudyObjectMarkupExtension(DependencyProperty property, string path)
        {
            Prefix = property.Name;
            Path = path;
        }
        private StudyObjectMarkupExtension(string prefix, string path)
        {
            Prefix = prefix;
            Path = path;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;

            if (target.TargetObject?.GetType().FullName == "System.Windows.SharedDp")
                return new Duplicate(this);

            return CreateBinding(target).ProvideValue(serviceProvider);
        }

        internal Binding CreateBinding(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            return CreateBinding(target);
        }
        internal Binding CreateBinding(IProvideValueTarget target)
        {
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

            // TODO: we should deal with the target not being in the tree yet (i.e. a template is being instantiated, target.Parent = null and RelativeSource fails)
            // however in the most common case, ShowStimuli, the templated object is a StudyStep itself and we can rely on its DataContext
            return new Binding(path) { RelativeSource = this is Duplicate ? null : source };
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
