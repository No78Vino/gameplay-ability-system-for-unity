using System;
using System.Collections.Generic;

namespace GAS.General
{
    /// <summary>
    /// 按优先级取值
    /// <para>可以动态修改默认优先级和默认值, 适用于延迟初始化的情况</para>
    /// <para>通过设置默认值, 配合RemoveAll()使用可立刻恢复默认值</para>
    /// <para>一般来说默认值的优先级应该是最低的, 但是允许默认值的优先级高于其他值</para>
    /// <para>优先级可以相同, 则取优先级为: 默认值 > 先添加的值 > 后添加的值</para>
    /// </summary>
    public sealed class PriorityValue<T> : IDisposable
    {
#if false //C# 10.0+
        public readonly record struct Data(string Key, int Priority, T Value);
#else
        public record Data(string Key, int Priority, T Value)
        {
            public string Key { get; } = Key;
            public int Priority { get; } = Priority;
            public T Value { get; } = Value;
        }
#endif

        private readonly List<Data> _datas; // 按优先级降序排列
        private readonly object _lock;

        public string CurrentKey
        {
            get
            {
                lock (_lock)
                {
                    return _currentData.Key;
                }
            }
        }

        public int CurrentPriority
        {
            get
            {
                lock (_lock)
                {
                    return _currentData.Priority;
                }
            }
        }

        public T CurrentValue
        {
            get
            {
                lock (_lock)
                {
                    return _currentData.Value;
                }
            }
        }

        public string DefaultKey => "@default";

        public int DefaultPriority
        {
            get
            {
                lock (_lock)
                {
                    return _defaultData.Priority;
                }
            }
        }

        public T DefaultValue
        {
            get
            {
                lock (_lock)
                {
                    return _defaultData.Value;
                }
            }
        }

        private Data _defaultData;
        private Data _currentData;

        public delegate void PostValueChanged(T oldValue, T newValue);

        private event PostValueChanged OnPostValueChanged;

        public PriorityValue() : this(default)
        {
        }

        public PriorityValue(T defaultValue) :
            this(int.MinValue, defaultValue)
        {
        }

        public PriorityValue(int defaultPriority, T defaultValue)
        {
            _defaultData = new(DefaultKey, defaultPriority, defaultValue);
            _currentData = _defaultData;
            _datas = new();
            _lock = new();
        }

        public T Add(string key, int priority, T value)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

                var index = _datas.FindIndex(data => data.Key == key);
                if (index < 0)
                {
                    Insert(key, priority, value);
                    Refresh();
                }
                else
                {
                    throw new ArgumentException($"Key {key} already exists.");
                }

                return CurrentValue;
            }
        }

        public T AddOrSet(string key, int priority, T value)
        {
            lock (_lock)
            {
                if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

                var index = _datas.FindIndex(data => data.Key == key);
                if (index >= 0)
                {
                    if (_datas[index].Priority == priority)
                    {
                        _datas[index] = new(key, priority, value);
                    }
                    else
                    {
                        _datas.RemoveAt(index);
                        Insert(key, priority, value);
                    }
                }
                else
                {
                    Insert(key, priority, value);
                }

                Refresh();

                return CurrentValue;
            }
        }

        private void Insert(string key, int priority, T value)
        {
            var data = new Data(key, priority, value);
            var insertIndex = _datas.Count;
            for (var i = 0; i < _datas.Count; i++)
            {
                if (priority > _datas[i].Priority)
                {
                    insertIndex = i;
                    break;
                }
            }

            _datas.Insert(insertIndex, data);
        }

        public T SetDefault(int priority, T value)
        {
            lock (_lock)
            {
                if (_defaultData.Priority != priority || !EqualityComparer<T>.Default.Equals(_defaultData.Value, value))
                {
                    _defaultData = new(DefaultKey, priority, value);
                    Refresh();
                }

                return CurrentValue;
            }
        }

        public T SetDefaultPriority(int priority)
        {
            return SetDefault(priority, DefaultValue);
        }

        public T SetDefaultValue(T value)
        {
            return SetDefault(DefaultPriority, value);
        }

        public T Remove(string key)
        {
            lock (_lock)
            {
                var index = _datas.FindIndex(data => data.Key == key);
                if (index >= 0)
                {
                    _datas.RemoveAt(index);
                    Refresh();
                }

                return CurrentValue;
            }
        }

        public T RemoveAll()
        {
            lock (_lock)
            {
                if (_datas.Count > 0)
                {
                    _datas.Clear();
                    Refresh();
                }

                return CurrentValue;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _currentData = _defaultData;
                _datas.Clear();
                OnPostValueChanged = null;
            }
        }

        private void Refresh()
        {
            var oldValue = _currentData.Value;

            _currentData = _defaultData;

            if (_datas.Count > 0)
            {
                var data = _datas[0];
                if (data.Priority > _defaultData.Priority)
                {
                    _currentData = data;
                }
            }

            if (EqualityComparer<T>.Default.Equals(oldValue, _currentData.Value) == false)
            {
                OnPostValueChanged?.Invoke(oldValue, _currentData.Value);
            }
        }

        public void RegisterPostValueChanged(PostValueChanged listener)
        {
            lock (_lock)
            {
                OnPostValueChanged += listener;
            }
        }

        public void UnregisterPostValueChanged(PostValueChanged listener)
        {
            lock (_lock)
            {
                OnPostValueChanged -= listener;
            }
        }
    }

    /// <summary>
    /// 通过Enable控制PriorityValue的某个key是否生效, 省去了手动管理Add/Remove等繁琐操作
    /// </summary>
    public sealed class PriorityValueToggle<T> : IDisposable
    {
        private PriorityValue<T>.Data _data;
        public PriorityValue<T> PriorityValue { get; }

        public string Key => _data.Key;

        public int Priority
        {
            get => _data.Priority;
            set
            {
                if (_data.Priority != value)
                {
                    _data = new(Key, value, Value);
                    if (IsEnabled)
                    {
                        PriorityValue?.AddOrSet(Key, Priority, Value);
                    }
                }
            }
        }

        public T Value
        {
            get => _data.Value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_data.Value, value))
                {
                    _data = new(Key, Priority, value);
                    if (IsEnabled)
                    {
                        PriorityValue?.AddOrSet(Key, Priority, Value);
                    }
                }
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    if (_isEnabled) PriorityValue?.AddOrSet(Key, Priority, Value);
                    else PriorityValue?.Remove(Key);
                }
            }
        }

        public PriorityValueToggle(PriorityValue<T> priorityValue, string key, int priority, T value)
        {
            PriorityValue = priorityValue;
            _data = new(key, priority, value);
        }

        public void Dispose()
        {
            IsEnabled = false;
        }
    }
}