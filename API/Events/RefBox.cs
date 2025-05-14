using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Events
{
    public unsafe class RefBox<T>
    {
        private T* _ptr;

        public RefBox(ref T value)
        {
            fixed (T* ptr = &value)
            {
                _ptr = ptr;
            }
        }

        public T Value
        {
            get => *_ptr;

            set => *_ptr = value;
        }
    }
}
