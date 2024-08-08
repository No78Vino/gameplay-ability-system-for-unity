using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace GAS.General
{
    public class ArrayPool<T>
    {
        private readonly int _maxCapacity;
        private readonly ConcurrentDictionary<int, Pool> _items = new();

        public ArrayPool(int maxCapacity = 1024)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "maxCapacity must be greater than 0");
            }

            _maxCapacity = maxCapacity;
        }

        public T[] Fetch(int length)
        {
            return _items.TryGetValue(length, out var pool) ? pool.Get() : new T[length];
        }

        public bool Recycle(T[] obj)
        {
            if (obj == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("ArrayPool<T>.Recycle: obj is null");
#endif
            }
            else
            {
                if (!_items.TryGetValue(obj.Length, out var queue))
                {
                    Profiler.BeginSample("ArrayPoolEx<T>.CreateInstance");
                    queue = _items[obj.Length] = new Pool(obj.Length, _maxCapacity);
                    Profiler.EndSample();
                }

                return queue.Return(obj);
            }

            return false;
        }

        private class Pool
        {
            private readonly int _arrayLength;
            private readonly int _maxCapacity;
            private int _numItems;
            private readonly ConcurrentQueue<T[]> _items = new();
            private T[] _fastItem;

            public Pool(int arrayLength, int maxCapacity = 128)
            {
                if (_arrayLength < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayLength), "arrayLength must be greater than or equal to 0");
                }

                if (maxCapacity <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxCapacity), "maxCapacity must be greater than 0");
                }

                _arrayLength = arrayLength;
                _maxCapacity = maxCapacity;
            }

            public T[] Get()
            {
                var item = _fastItem;
                if (item == null || Interlocked.CompareExchange(ref _fastItem, null, item) != item)
                {
                    if (_items.TryDequeue(out item))
                    {
                        Interlocked.Decrement(ref _numItems);
                    }
                    else
                    {
                        Profiler.BeginSample("ArrayPool<T>.Pool<TT>.CreateInstance");
                        item = new T[_arrayLength];
                        Profiler.EndSample();
                    }
                }

                return item;
            }

            public bool Return(T[] obj)
            {
                if (obj == null)
                {
                    return false;
                }

                if (obj.Length != _arrayLength)
                {
                    Debug.LogWarning($"Array length({obj.Length}) not match({_arrayLength})");
                    return false;
                }

                if (_fastItem != null || Interlocked.CompareExchange(ref _fastItem, obj, null) != null)
                {
                    if (Interlocked.Increment(ref _numItems) <= _maxCapacity)
                    {
                        _items.Enqueue(obj);
                        return true;
                    }

                    Interlocked.Decrement(ref _numItems);
#if UNITY_EDITOR
                    Debug.LogWarning($"ArrayPool<{typeof(T).FullName}>.Return: Exceed max capacity({_maxCapacity}), consider increase max capacity.");
#endif
                }

                return false;
            }
        }
    }
}