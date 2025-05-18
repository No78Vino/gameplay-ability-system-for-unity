using Unity.Entities;

namespace GAS.Runtime
{
    public static class EntityHelper
    {
        private static EntityManager _entityManager => GASManager.EntityManager;
        private static EntityCommandBuffer _ecb;
        private static bool _usingEcb = false;
        
        public static void RegisterEntityCommandBuffer(EntityCommandBuffer ecb)
        {
            _ecb = ecb;
            _usingEcb = true;
        }
        
        public static void UnregisterEntityCommandBuffer()
        {
            _ecb = default;
            _usingEcb = false;
        }
        
        public static void AddComponent<T>(Entity entity) where T : unmanaged,IComponentData
        {
            if(_usingEcb)
                _ecb.AddComponent<T>(entity);
            else
                _entityManager.AddComponent<T>(entity);
        }
        
        public static void AddManagedComponent<T>(Entity entity) where T : class,IComponentData
        {
            if(_usingEcb)
                _ecb.AddComponent<T>(entity);
            else
                _entityManager.AddComponent<T>(entity);
        }
        
        public static void RemoveComponent<T>(Entity entity) where T : IComponentData
        {
            if(_usingEcb)
                _ecb.RemoveComponent<T>(entity);
            else
                _entityManager.RemoveComponent<T>(entity);
        }
        
        public static void SetComponent<T>(Entity entity, T component) where T : unmanaged,IComponentData
        {
            if(_usingEcb)
                _ecb.SetComponent(entity,component);
            else
                _entityManager.SetComponentData(entity,component);
        }
        
        public static void SetManagedComponent<T>(Entity entity, T component) where T : class,IComponentData,new()
        {
            if(_usingEcb)
                _ecb.SetComponent(entity,component);
            else
                _entityManager.SetComponentData(entity,component);
        }
        
        public static void DestroyEntity(Entity entity)
        {
            if(_usingEcb)
                _ecb.DestroyEntity(entity);
            else
                _entityManager.DestroyEntity(entity);
        }
        
        public static void SetComponentEnabled<T>(Entity entity, bool enable) where T : IEnableableComponent
        {
            _entityManager.SetComponentEnabled<T>(entity,enable);
        }
    }
}