using System;
using System.Runtime.InteropServices;

namespace TerraJS.JSEngine.API.Events.Ref
{
    public unsafe class RefBox<T>(T value) : IDisposable
    {
        private GCHandle _handle = GCHandle.Alloc(value, GCHandleType.Normal);
        
        private bool _disposed;

        public T Value
        {
            get => _disposed || !_handle.IsAllocated ? throw new ObjectDisposedException(GetType().FullName) : (T)_handle.Target;

            set
            {
                ObjectDisposedException.ThrowIf(_disposed || !_handle.IsAllocated, this);

                _handle.Target = value;
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_handle.IsAllocated)
                    _handle.Free();

                GC.SuppressFinalize(this);

                _disposed = true;
            }
        }

        ~RefBox() => Dispose();
    }
}
