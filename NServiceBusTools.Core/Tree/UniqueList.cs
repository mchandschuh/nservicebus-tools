using System.Collections;
using System.Collections.Generic;

namespace NServiceBusTools.Tree
{
    /// <summary>
    /// Provides an implementation of <see cref="IList{T}"/> that enforces uniqueness and order
    /// </summary>
    /// <typeparam name="T">The element type</typeparam>
    public class UniqueList<T> : IList<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly HashSet<T> _set = new HashSet<T>();

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return _list[index]; }
            set { _list.Insert(index, value); }
        }

        public void Add(T item)
        {
            TryAdd(item);
        }

        public bool TryAdd(T item)
        {
            if (_set.Add(item))
            {
                _list.Add(item);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _set.Clear();
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (_set.Remove(item))
            {
                _list.Remove(item);
                return true;
            }
            return false;
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}