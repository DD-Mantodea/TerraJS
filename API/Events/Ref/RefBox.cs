using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraJS.API.Events.Ref
{
    public unsafe class RefBox<T>(T* ptr)
    {
        private readonly T* _ptr = ptr;

        public T Value
        {
            get => *_ptr;

            set => *_ptr = value;
        }
    }
}
