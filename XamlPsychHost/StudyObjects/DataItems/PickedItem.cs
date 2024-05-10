using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace HurlbertVisionLab.XamlPsychHost
{
    public class PickedItem : IReadOnlyList<object>, ILogInfo, IStructuralEquatable
    {
        private readonly object[] _items;
        private readonly int[] _indices;

        public PickedItem(object[] items, int[] indices)
        {
            _items = items;
            _indices = (int[])indices.Clone();
        }

        public object this[int index] => _items[index];
        public int Count => _items.Length;

        public int[] Indices => _indices;

        public IEnumerator<object> GetEnumerator() => ((IEnumerable<object>)_items).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public string ToLogString(StudyContext context)
        {
            return context.ToLogStringWithFallback(_items, _indices);
        }

        public override bool Equals(object obj) => Equals(obj as PickedItem);
        public bool Equals(PickedItem other)
        {
            if (other == null)
                return false;

            return 
                this._indices.SequenceEqual(other._indices) &&
                this._items.SequenceEqual(other._items);
        }

        public bool Equals(object obj, IEqualityComparer comparer)
        {
            if (obj is not PickedItem other)
                return false;

            IStructuralEquatable indices = _indices;
            if (!indices.Equals(other._indices, comparer))
                return false;

            IStructuralEquatable items = _items;
            if (!items.Equals(other._items, comparer))
                return false;

            return true;
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            IStructuralEquatable indices = _indices;
            IStructuralEquatable items = _items;

            int hashCode = 961649469;
            hashCode = hashCode * -1521134295 + indices.GetHashCode(comparer);
            hashCode = hashCode * -1521134295 + items.GetHashCode(comparer);
            return hashCode;
        }
    }
}
