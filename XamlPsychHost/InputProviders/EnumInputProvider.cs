using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace HurlbertVisionLab.XamlPsychHost
{
    [ContentProperty(nameof(Mappings))]
    public abstract class EnumInputProvider<T, TMap> : IInputProvider where T : Enum where TMap : IEnumMap<T>
    {
        public List<TMap> Mappings { get; } = new List<TMap>();
        private Dictionary<T, List<TMap>> _mappingsLookup;

        protected IStudyInputSource InputSource { get; private set; }

        public void InitializeLookup()
        {
            _mappingsLookup = new Dictionary<T, List<TMap>>(Mappings.Count);
            foreach (TMap map in Mappings)
            {
                if (!_mappingsLookup.TryGetValue(map.Key, out List<TMap> mappings))
                    _mappingsLookup[map.Key] = mappings = new List<TMap>();

                mappings.Add(map);
            }
        }

        protected bool ReportInput(T key)
        {
            if (InputSource is DispatcherObject source && source.Dispatcher.Thread != Thread.CurrentThread)
                return source.Dispatcher.Invoke(() => ReportInput(key));

            if (_mappingsLookup.TryGetValue(key, out List<TMap> mappings))
            {
                foreach (TMap mapping in mappings)
                    InputSource.ReportStudyInput(this, mapping.ToInput);

                return true;
            }

            InputSource.ReportStudyInput(this, key.ToString());
            return false;
        }

        public void Map(MapInput map)
        {
            if (map is TMap tmap)
                Mappings.Add(tmap);

            TMap key = CreateMap(map);
            Mappings.Add(key);
        }

        protected abstract TMap CreateMap(MapInput map);

        public void BindTo(IStudyInputSource source)
        {
            InputSource = source;
            InitializeLookup();

            Bind();
        }
        protected abstract void Bind();
        public abstract void Unbind();
    }
}
