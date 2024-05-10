using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class CombinedItem : IReadOnlyDictionary<string, object>, ILogInfo, IStructuralEquatable
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

        public bool Equals(object obj, IEqualityComparer comparer)
        {
            if (obj is not CombinedItem other)
                return false;

            IStructuralEquatable keys = _keys;
            if (!keys.Equals(other._keys, comparer))
                return false;
            
            IStructuralEquatable values = _values;
            if (!values.Equals(other._values, comparer))
                return false;

            return true;
        }
        public int GetHashCode(IEqualityComparer comparer)
        {
            IStructuralEquatable keys = _keys;
            IStructuralEquatable values = _values;

            int hashCode = 1361046294;
            hashCode = hashCode * -1521134295 + keys.GetHashCode(comparer);
            hashCode = hashCode * -1521134295 + values.GetHashCode(comparer);
            return hashCode;
        }
    }
}
