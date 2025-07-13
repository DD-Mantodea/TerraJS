using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Events.Ref
{
    public unsafe class RefBoxArray<T>(T* ptr, int length) : IEnumerable<RefBox<T>>
    {
        private readonly T* _ptr = ptr;

        private readonly int _length = length;

        public RefBox<T> Get(int index)
        {
            if (index < 0 || index >= _length)
                return default;

            return new(_ptr + index);
        }

        public void Set(int index, T value)
        {
            if (index < 0 || index >= _length)
                return;

            *(_ptr + index) = value;
        }

        public int GetFirst(Func<T, bool> predicate)
        {
            for (int i = 0; i < _length; i++)
            {
                RefBox<T> box = Get(i);
                if (predicate(box.Value))
                    return i;
            }

            return -1;
        }

        public IEnumerator<RefBox<T>> GetEnumerator()
        {
            for (int i = 0; i < _length; i++)
                yield return Get(i);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
