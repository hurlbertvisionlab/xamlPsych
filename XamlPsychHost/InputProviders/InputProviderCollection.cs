using System;
using System.Collections.ObjectModel;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class InputProviderCollection : KeyedCollection<Type, IInputProvider>
    {
        protected override Type GetKeyForItem(IInputProvider item) => item.GetType();

        public IInputProvider GetOrCreate(Type providerType)
        {
            if (Contains(providerType))
                return this[providerType];

            IInputProvider provider = (IInputProvider)providerType.GetConstructor(new Type[0]).Invoke(null);
            Add(provider);
            return provider;
        }

        internal void BindTo(IStudyInputSource source)
        {
            foreach (IInputProvider provider in this)
                provider.BindTo(source);
        }
    }
}
