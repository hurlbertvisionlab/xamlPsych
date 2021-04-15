using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace HurlbertVisionLab.XamlPsychHost
{
    public enum CombiningMode
    {
         AllCombinations,
         Parallel
    }

    public class CombiningSource : ResourceSource
    {
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register(nameof(Mode), typeof(CombiningMode), typeof(CombiningSource));

        public CombiningMode Mode
        {
            get { return (CombiningMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }


        public override IEnumerable<object> GenerateItems(StudyContext context)
        {
            object[] resources = new object[Keys.Count];
            for (int i = 0; i < Keys.Count; i++)
                if ((resources[i] = context.Study.FindResource(Keys[i])) is DataSourceProvider provider)
                    resources[i] = provider.Data;

            switch (Mode)
            {
                case CombiningMode.AllCombinations:
                    return GenerateItemsAll(context, resources);

                case CombiningMode.Parallel:
                    return GenerateItemsParallel(context, resources);

                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<object> GenerateItemsAll(StudyContext context, params object[] resources)
        {
            object[][] valueArrays = new object[resources.Length][];
            for (int i = 0; i < resources.Length; i++)
            {
                if (resources[i] is StudyDataSource source)
                    valueArrays[i] = source.GenerateItems(context).ToArray();

                else if (resources[i] is ICollection collection)
                {
                    valueArrays[i] = new object[collection.Count];
                    collection.CopyTo(valueArrays[i], 0);
                }

                else
                    valueArrays[i] = new object[] { resources[i] };
            }

            int[] lengths = new int[resources.Length];
            for (int i = 0; i < lengths.Length; i++)
            {
                lengths[i] = valueArrays[i].Length;
                if (lengths[i] < 1) // if any of the sources does not have any items, we cannot generate any combination
                    yield break;
            }

            int[] indices = new int[resources.Length];
            while (true)
            {
                object[] values = new object[resources.Length];
                for (int i = 0; i < indices.Length; i++)
                    values[i] = valueArrays[i][indices[i]];

                yield return new CombinedItem(Keys, values);

                int j = indices.Length - 1;
                while (true)
                {
                    bool carry = AddOne(indices, lengths, j);
                    if (!carry)
                        break;

                    else if (--j < 0)
                        yield break;
                }
            }
        }

        private bool AddOne(int[] indices, int[] lengths, int i)
        {
            if (++indices[i] >= lengths[i])
            {
                for (int j = i; j < indices.Length; j++)
                    indices[j] = 0;

                return true;
            }

            return false;
        }

        private IEnumerable<object> GenerateItemsParallel(StudyContext context, params object[] resources)
        {
            IEnumerator[] enumerators = new IEnumerator[resources.Length];
            for (int i = 0; i < enumerators.Length; i++)
            {
                if (resources[i] is StudyDataSource source)
                    enumerators[i] = source.GenerateItems(context).GetEnumerator();

                else if (resources[i] is ICollection collection)
                    enumerators[i] = collection.GetEnumerator();

                else
                    enumerators[i] = new object[] { resources[i] }.GetEnumerator();
            }

            bool[] nexts = new bool[resources.Length];
            for (int i = 0; i < nexts.Length; i++)
                nexts[i] = true;

            while (true)
            {
                object[] values = new object[resources.Length];

                for (int i = 0; i < enumerators.Length; i++)
                    if (nexts[i])
                        if (enumerators[i].MoveNext())
                            values[i] = enumerators[i].Current;
                        else
                            nexts[i] = false;

                if (nexts.Any(b => b))
                    yield return new CombinedItem(Keys, values);
                else
                    yield break;
            }
        }

        public override int? GetItemsCount(StudyContext context)
        {
            switch (Mode)
            {
                case CombiningMode.AllCombinations:
                    return GetItemsCountAll(context);

                case CombiningMode.Parallel:
                    return GetItemsCountParallel(context);

                default:
                    throw new NotImplementedException();
            }
        }

        private int? GetItemsCountAll(StudyContext context)
        {
            int? count = 1;

            foreach (string key in Keys)
            {
                object resource = context.Study.FindResource(key);
                if (resource is DataSourceProvider provider)
                    resource = provider.Data;

                if (resource is StudyDataSource source)
                    count *= source.GetItemsCount(context);

                else if (resource is ICollection collection)
                    count *= collection.Count;
            }

            return count;
        }

        private int? GetItemsCountParallel(StudyContext context)
        {
            int count = 0;

            foreach (string key in Keys)
            {
                object resource = context.Study.FindResource(key);
                if (resource is DataSourceProvider provider)
                    resource = provider.Data;

                if (resource is StudyDataSource source)
                    if (source.GetItemsCount(context) is int sourceCount)
                        count = Math.Max(count, sourceCount);
                    else
                        return null;

                else if (resource is ICollection collection)
                    count = Math.Max(count, collection.Count);

                else if (count < 1)
                    count = 1;
            }

            return count;
        }

    }


    public class CombinedItem : IReadOnlyDictionary<string, object>, ILogInfo
    {
        private readonly string[] _keys;
        private readonly object[] _values;

        public CombinedItem(ICollection<string> keys, object[] values)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            if (values == null) throw new ArgumentNullException(nameof(keys));

            if (keys.Count != values.Length)
                throw new ArgumentException();
            
            _keys = keys.ToArray();
            _values = values;
        }

        public object this[string key]
        {
            get
            {
                for (int i = 0; i < _keys.Length; i++)
                    if (_keys[i] == key)
                        return _values[i];

                return null;
            }
        }
        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < _values.Length)
                    return _values[index];

                return null;
            }
        }

        public int Count => _keys.Length;
        public IEnumerable<string> Keys => _keys;
        public IEnumerable<object> Values => _values;

        public bool ContainsKey(string key) => Array.IndexOf(_keys, key) >= 0;
        public bool TryGetValue(string key, out object value) => (value = this[key]) != null;

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            for (int i = 0; i < _keys.Length; i++)
                yield return new KeyValuePair<string, object>(_keys[i], _values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            if (_keys.Length < 1)
                return base.ToString();

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < _keys.Length; i++)
                s.AppendFormat("{0}={1}, ", _keys[i], _values[i]);

            s.Remove(s.Length - 2, 2);
            return s.ToString();
        }

        public string ToLogString(StudyContext context)
        {
            if (_keys.Length < 1)
                return null;

            StringBuilder s = new StringBuilder();
            for (int i = 0; i < _keys.Length; i++)
                s.AppendFormat("{0},", context.ToLogString(_values[i]));

            s.Remove(s.Length - 1, 1);
            return s.ToString();
        }
    }
}
