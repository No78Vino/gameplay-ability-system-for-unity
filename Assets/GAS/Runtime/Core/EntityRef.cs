using System;

namespace GAS.Runtime
{
    public static class IdGenerator
    {
        private static ulong _id;

        /// <summary>
        /// 从 1 开始递增的唯一 ID, 0 用于表示无效 ID
        /// </summary>
        public static ulong Next => ++_id;
    }

    public interface IEntity
    {
        ulong InstanceId { get; }
    }

    public struct EntityRef<T> where T : class, IEntity
    {
        private readonly ulong _instanceId;
        private T _entity;

        public EntityRef(T t)
        {
            if (t == null)
            {
                _instanceId = 0;
                _entity = null;
                return;
            }

            if (t.InstanceId == 0)
            {
                throw new ArgumentException("EntityRef: Entity must have a valid instance id");
            }

            _instanceId = t.InstanceId;
            _entity = t;
        }

        public T Value
        {
            get
            {
                if (_entity == null) return null;

                if (_entity.InstanceId != _instanceId) _entity = null;

                return _entity;
            }
        }

        public static implicit operator EntityRef<T>(T t) => new(t);

        public static implicit operator T(EntityRef<T> v) => v.Value;
    }
}