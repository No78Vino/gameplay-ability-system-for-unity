using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class EntityHelper
    {
        private static EntityCommandBuffer _ecb;
        private static bool _usingEcb;
        private static EntityManager _entityManager => GASManager.EntityManager;

        // 新增:记录通过ECB添加的Component类型
        private static Dictionary<(Entity, Type), bool> _pendingComponents = new();
    
        // 新增:记录延迟的SetComponent命令
        private static List<Action> _deferredSetCommands = new();

        // 新增:缓存待设置的Component数据
        private static Dictionary<(Entity, Type), object> _pendingComponentData = new();
        
        /// <summary>
        ///     注册ECB
        /// </summary>
        /// <param name="allocator"></param>
        [BurstCompile]
        public static EntityCommandBuffer RegisterEntityCommandBuffer(Allocator allocator = Allocator.Temp)
        {
            _ecb = new EntityCommandBuffer(allocator);
            _usingEcb = true;
            _pendingComponents.Clear();
            _deferredSetCommands.Clear();
            _pendingComponentData.Clear();
            return _ecb;
        }
        
        /// <summary>
        ///     注销ECB
        /// </summary>
        [BurstCompile]
        public static void UnregisterEntityCommandBuffer()
        {
            _ecb = default;
            _usingEcb = false;
            _pendingComponents.Clear();
            _deferredSetCommands.Clear();
            _pendingComponentData.Clear();
        }

        /// <summary>
        ///     新增:在Playback后执行延迟命令
        /// </summary>
        public static void ExecuteDeferredCommands()
        {
            foreach (var command in _deferredSetCommands)
                command();
            
            _deferredSetCommands.Clear();
            _pendingComponentData.Clear();
        }
        
        /// <summary>
        /// 获取非托管组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentData<T>(Entity entity) where T : unmanaged, IComponentData
        {
            if (_usingEcb && _pendingComponentData.ContainsKey((entity, typeof(T))))
            {
                // 返回缓存的数据
                return (T)_pendingComponentData[(entity, typeof(T))];
            }
        
            return _entityManager.GetComponentData<T>(entity);
        }

        /// <summary>
        /// 获取非托管组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetManagedComponentData<T>(Entity entity) where T : class, IComponentData, new()
        {
            if (_usingEcb && _pendingComponentData.ContainsKey((entity, typeof(T))))
            {
                return (T)_pendingComponentData[(entity, typeof(T))];
            }
        
            return _entityManager.GetComponentData<T>(entity);
        }
        
        public static DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : unmanaged, IBufferElementData
        {
            return _entityManager.GetBuffer<T>(entity);
        }
        
        public static void AddBuffer<T>(Entity entity) where T : unmanaged, IBufferElementData
        {
            _entityManager.AddBuffer<T>(entity);
        }

        public static bool HasComponent<T>(Entity entity) where T : unmanaged, IComponentData
        {
            return _entityManager.HasComponent<T>(entity);
        }
        
        public static bool HasManagedComponent<T>(Entity entity) where T : class, IComponentData
        {
            return _entityManager.HasComponent<T>(entity);
        }
        
        /// <summary>
        ///     添加非托管组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddComponent<T>(Entity entity) where T : unmanaged, IComponentData
        {
            if (_usingEcb)
            {
                _ecb.AddComponent<T>(entity);
                // 记录这个Component是通过ECB添加的
                _pendingComponents[(entity, typeof(T))] = true;
            }
            else
            {
                _entityManager.AddComponent<T>(entity);
            }
        }

        /// <summary>
        ///     添加托管组件
        /// </summary>
        /// <param name="entity"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddManagedComponent<T>(Entity entity) where T : class, IComponentData
        {
            if (_usingEcb)
            {
                _ecb.AddComponent<T>(entity);
                _pendingComponents[(entity, typeof(T))] = true;
            }
            else
            {
                _entityManager.AddComponent<T>(entity);
            }
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
            {
                if (_pendingComponents.ContainsKey((entity, typeof(T))))
                {
                    // 缓存数据供GetComponentData使用
                    _pendingComponentData[(entity, typeof(T))] = component;
                
                    // 延迟执行
                    _deferredSetCommands.Add(() => 
                    {
                        if (_entityManager.Exists(entity) && _entityManager.HasComponent<T>(entity))
                        {
                            _entityManager.SetComponentData(entity, component);
                        }
                    });
                }
                else
                {
                    _ecb.SetComponent(entity, component);
                }
            }
            else
            {
                _entityManager.SetComponentData(entity, component);
            }
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
            {
                if (_pendingComponents.ContainsKey((entity, typeof(T))))
                {
                    _pendingComponentData[(entity, typeof(T))] = component;
                
                    _deferredSetCommands.Add(() => 
                    {
                        if (_entityManager.Exists(entity) && _entityManager.HasComponent<T>(entity))
                        {
                            _entityManager.SetComponentData(entity, component);
                        }
                    });
                }
                else
                {
                    _ecb.SetComponent(entity, component);
                }
            }
            else
            {
                _entityManager.SetComponentData(entity, component);
            }
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

        public static string GetEntityName(Entity entity)
        {
            if (!GASManager.IsInitialized) return string.Empty;
  
            string name = GASManager.EntityManager.GetName(entity);
            if (string.IsNullOrEmpty(name)) name =  entity.ToString();
            return name;
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
            if (GASManager.ExWorld != null && GASManager.ExWorld.IsCreated   
                                           && _entityManager.Exists(entity))  
                _bindingGameObjects.Remove(entity);  
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