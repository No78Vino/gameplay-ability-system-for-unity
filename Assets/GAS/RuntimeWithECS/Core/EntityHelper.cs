using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class EntityHelper
    {
        private static EntityCommandBuffer _ecb;
        private static bool _usingEcb;
        private static EntityManager _entityManager => GASManager.EntityManager;

        /// <summary>
        ///     注册ECB
        /// </summary>
        /// <param name="ecb"></param>
        public static void RegisterEntityCommandBuffer(EntityCommandBuffer ecb)
        {
            _ecb = ecb;
            _usingEcb = true;
        }

        /// <summary>
        ///     注销ECB
        /// </summary>
        public static void UnregisterEntityCommandBuffer()
        {
            _ecb = default;
            _usingEcb = false;
        }

        /// <summary>
        ///     添加非托管组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddComponent<T>(Entity entity) where T : unmanaged, IComponentData
        {
            if (_usingEcb)
                _ecb.AddComponent<T>(entity);
            else
                _entityManager.AddComponent<T>(entity);
        }

        /// <summary>
        ///     添加托管组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddManagedComponent<T>(Entity entity) where T : class, IComponentData
        {
            if (_usingEcb)
                _ecb.AddComponent<T>(entity);
            else
                _entityManager.AddComponent<T>(entity);
        }

        /// <summary>
        ///     移除组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveComponent<T>(Entity entity) where T : IComponentData
        {
            if (_usingEcb)
                _ecb.RemoveComponent<T>(entity);
            else
                _entityManager.RemoveComponent<T>(entity);
        }

        /// <summary>
        ///     设置非托管组件数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetComponent<T>(Entity entity, T component) where T : unmanaged, IComponentData
        {
            if (_usingEcb)
                _ecb.SetComponent(entity, component);
            else
                _entityManager.SetComponentData(entity, component);
        }

        /// <summary>
        ///     设置托管组件数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetManagedComponent<T>(Entity entity, T component) where T : class, IComponentData, new()
        {
            if (_usingEcb)
                _ecb.SetComponent(entity, component);
            else
                _entityManager.SetComponentData(entity, component);
        }
        
        /// <summary>
        ///     摧毁实例
        /// </summary>
        /// <param name="entity"></param>
        public static void DestroyEntity(Entity entity)
        {
            if (_usingEcb)
                _ecb.DestroyEntity(entity);
            else
                _entityManager.DestroyEntity(entity);
        }
        
        /// <summary>
        ///   实例化实体
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Entity Instantiate(Entity entity)
        {
            return _usingEcb ? _ecb.Instantiate(entity) : _entityManager.Instantiate(entity);
        }

        /// <summary>
        ///     设置能变组件状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enable"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetComponentEnabled<T>(Entity entity, bool enable) where T :struct, IEnableableComponent
        {
            if (_usingEcb)
                _ecb.SetComponentEnabled<T>(entity, enable);
            else
                _entityManager.SetComponentEnabled<T>(entity, enable);
        }
        
        /// <summary>
        ///     设置能变组件状态
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="enable"></param>
        /// <typeparam name="T"></typeparam>
        public static void SetComponentManagedEnabled<T>(Entity entity, bool enable) where T :class, IEnableableComponent,new()
        {
            if (_usingEcb)
                _ecb.SetComponentEnabled<T>(entity, enable);
            else
                _entityManager.SetComponentEnabled<T>(entity, enable);
        }

        public static void SetName(Entity entity, string name)
        {
            if (_usingEcb)
                _ecb.SetName(entity, name);
            else
                _entityManager.SetName(entity, name);
        }

        #region GameObject绑定

        private static readonly Dictionary<Entity, GameObject> _bindingGameObjects = new();

        public static void ClearGameObjectBinding()
        {
            _bindingGameObjects.Clear();
        }

        /// <summary>
        ///     绑定gameObject到entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="gameObject"></param>
        public static void BindGameObjectToEntity(Entity entity, GameObject gameObject)
        {
            if (_entityManager.Exists(entity) && gameObject != null) _bindingGameObjects.Add(entity, gameObject);
        }

        /// <summary>
        ///     解绑gameObject
        /// </summary>
        /// <param name="entity"></param>
        public static void UnbindGameObjectToEntity(Entity entity)
        {
            if (_entityManager.Exists(entity)) _bindingGameObjects.Remove(entity);
        }

        /// <summary>
        ///     获取entity绑定的gameObject
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static GameObject GetGameObjectFromEntity(Entity entity)
        {
            return _bindingGameObjects.GetValueOrDefault(entity);
        }

        #endregion
    }
}