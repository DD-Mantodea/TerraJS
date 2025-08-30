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

        public override bool Equals(object obj)
        {
            return obj is RefBox<T> box && box._ptr == _ptr;
        }
    }
}
