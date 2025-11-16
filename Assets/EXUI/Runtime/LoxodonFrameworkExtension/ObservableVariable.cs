using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Extension
{
    public class ObservableVariable<T> : ObservableObject
    {
        private T _value;

        public T Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
}