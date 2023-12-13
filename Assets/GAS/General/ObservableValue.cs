using System;

namespace GAS.General
{
    public class ObservableValue<T>
    {
        private T _value;

        public ObservableValue(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(oldValue, value);
            }
        }

        public event Action<T, T> OnValueChanged;
    }
}