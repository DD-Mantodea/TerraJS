using System;
using System.Collections;
using System.Collections.Generic;

namespace TerraJS.JSEngine.API.Events.Ref
{
    public unsafe class RefBoxArray<T>(T[] array) : IEnumerable<RefBox<T>>
    {
        private readonly T[] _array = array;

        public int Length => _array.Length;

        public bool IsReadOnly { get; set; }

        public T this[int index] 
        {
            get => Get(index);
        }

        public T Get(int index)
        {
            if (index < 0 || index >= Length)
                return default;

            return _array[index];
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= Length)
                return;

            _array[index] = value;
        }

        public int GetFirstIndex(Func<T, bool> predicate)
        {
            for (int i = 0; i < Length; i++)
                if (predicate(_array[i]))
                    return i;

            return -1;
        }

        public IEnumerator<RefBox<T>> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return new(_array[i]);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
