using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class MapInputCollection : Collection<MapInput>
    {
        private Study _study;

        public MapInputCollection(Study study)
        {
            _study = study;
        }

        protected override void InsertItem(int index, MapInput item)
        {
            base.InsertItem(index, item);

            LocalValueEnumerator localEnumerator = item.GetLocalValueEnumerator();
            while (localEnumerator.MoveNext())
            {
                Type propertyOwner = localEnumerator.Current.Property.OwnerType;
                if (propertyOwner.GetCustomAttribute<StudyInputProviderAttribute>() is StudyInputProviderAttribute attribute)
                    _study.InputProviders.GetOrCreate(attribute.Type).Map(item);
            }
        }
    }
}
