using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace GAS.General
{
    public interface IPool
    {
        bool IsFromPool { get; set; }
    }

    public class ObjectPool
    {
        private static ObjectPool _singleton;
        public static ObjectPool Instance => _singleton ??= new ObjectPool();

        private readonly ConcurrentDictionary<Type, Pool> _objPool = new();

        private readonly Func<Type, Pool> _addPoolFunc = type => new Pool(type, 1024);

        public T Fetch<T>() where T : class
        {
            var type = typeof(T);
            var obj = Fetch(type);
            return obj as T;
        }

        public object Fetch(Type type, bool isFromPool = true)
        {
            object obj;

            if (!isFromPool)
            {
                Profiler.BeginSample("ObjectPool.CreateInstance");
                obj = Activator.CreateInstance(type);
                Profiler.EndSample();
            }
            else
            {
                var pool = GetPool(type);
                obj = pool.Get();
                if (obj is IPool p)
                {
                    p.IsFromPool = true;
                }
            }

            return obj;
        }

        public void Recycle(object obj)
        {
            if (obj is IPool p)
            {
                if (!p.IsFromPool)
                {
                    return;
                }

                // 防止多次入池
                p.IsFromPool = false;
            }

            var type = obj.GetType();
            var pool = GetPool(type);
            pool.Return(obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Pool GetPool(Type type)
        {
            return _objPool.GetOrAdd(type, _addPoolFunc);
        }

        /// <summary>
        /// 线程安全的无锁对象池
        /// </summary>
        private class Pool
        {
            private readonly Type _objectType;
            private readonly int _maxCapacity;
            private int _numItems;
            private readonly ConcurrentQueue<object> _items = new();
            private object _fastItem;

            public Pool(Type objectType, int maxCapacity)
            {
                if (maxCapacity <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxCapacity), "maxCapacity must be greater than 0");
                }

                _objectType = objectType;
                _maxCapacity = maxCapacity;
            }

            public object Get()
            {
                object item = _fastItem;
                if (item == null || Interlocked.CompareExchange(ref _fastItem, null, item) != item)
                {
                    if (_items.TryDequeue(out item))
                    {
                        Interlocked.Decrement(ref _numItems);
                    }
                    else
                    {
                        Profiler.BeginSample("Pool.CreateInstance");
                        item = Activator.CreateInstance(_objectType);
                        Profiler.EndSample();
                    }
                }

                return item;
            }

            public bool Return(object obj)
            {
                if (_fastItem != null || Interlocked.CompareExchange(ref _fastItem, obj, null) != null)
                {
                    if (Interlocked.Increment(ref _numItems) <= _maxCapacity)
                    {
                        _items.Enqueue(obj);
                        return true;
                    }

                    Interlocked.Decrement(ref _numItems);
#if UNITY_EDITOR
                    Debug.LogWarning($"Pool<{_objectType.FullName}>.Return: Exceed max capacity({_maxCapacity}), consider increase max capacity.");
#endif
                }

                return false;
            }
        }
    }
}