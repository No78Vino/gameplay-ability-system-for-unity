using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GAS.Runtime
{
    /// <summary>
    /// 挂载Prefab的Cue
    /// 用于在宿主ASC上实例化并管理Prefab对象
    ///
    /// 功能特性：
    /// - 支持Prefab资源路径加载
    /// - 支持挂载点配置（根节点或指定子节点）
    /// - 支持位置/旋转/缩放偏移
    /// - 支持世界坐标或本地坐标模式
    /// - 支持Layer层级设置（含递归设置）
    /// - 支持SortingOrder/SortingLayer设置（2D渲染）
    /// - 支持跟随宿主或独立存在
    /// - 支持粒子系统自动播放/停止
    /// - 支持延迟销毁
    /// </summary>
    public class CueMountPrefab : GameplayCueBase<XParamMountPrefab>
    {
        private GameObject _instance;
        private Transform _mountPoint;
        private ParticleSystem[] _particleSystems;
        private bool _isDestroying;
        private float _destroyStartTime;
        private bool _pendingDestroy;

        #region 生命周期

        public override void OnAdd(float time)
        {
            base.OnAdd(time);

            var hostGo = _abilitySystemCell?.GameObject;
            if (hostGo == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("[CueMountPrefab] 宿主GameObject为空，无法挂载Prefab");
#endif
                return;
            }

            // 查找挂载点
            FindMountPoint(hostGo);
        }

        public override void OnActivate(float time)
        {
            base.OnActivate(time);

            if (_instance != null) return;

            // 加载并实例化Prefab
            InstantiatePrefab();
        }

        public override void OnTick(float time)
        {
            base.OnTick(time);

            // 处理延迟销毁
            if (_pendingDestroy && !_isDestroying)
            {
                if (time - _destroyStartTime >= Parameter.DestroyDelay)
                {
                    DestroyInstance();
                }
            }
        }

        public override void OnDeactivate(float time)
        {
            base.OnDeactivate(time);

            // 停止粒子系统
            if (Parameter.StopParticleOnDeactivate)
            {
                StopParticleSystems();
            }

            // 根据配置决定是否销毁
            if (Parameter.DestroyOnStop)
            {
                ScheduleDestroy(time);
            }
        }

        public override void OnRemove(float time)
        {
            base.OnRemove(time);
            DestroyInstance();
        }

        public override void OnDestroy(float time)
        {
            base.OnDestroy(time);
            DestroyInstance();
        }

        public override void Reset()
        {
            DestroyInstance();
        }

        #endregion

        #region 核心逻辑

        private void FindMountPoint(GameObject hostGo)
        {
            if (string.IsNullOrEmpty(Parameter.MountPointPath))
            {
                _mountPoint = hostGo.transform;
            }
            else
            {
                _mountPoint = hostGo.transform.Find(Parameter.MountPointPath);
                if (_mountPoint == null)
                {
#if UNITY_EDITOR
                    Debug.LogWarning($"[CueMountPrefab] 未找到挂载点: {Parameter.MountPointPath}，使用根节点");
#endif
                    _mountPoint = hostGo.transform;
                }
            }
        }

        private void InstantiatePrefab()
        {
            if (string.IsNullOrEmpty(Parameter.PrefabPath))
            {
#if UNITY_EDITOR
                Debug.LogWarning("[CueMountPrefab] Prefab路径为空");
#endif
                return;
            }

            // 加载Prefab
            var prefab = GASResourceLoader.LoadSync<GameObject>(Parameter.PrefabPath);
            if (prefab == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"[CueMountPrefab] 无法加载Prefab: {Parameter.PrefabPath}");
#endif
                return;
            }

            // 确定父节点
            Transform parent = null;
            if (Parameter.FollowHost && _mountPoint != null)
            {
                parent = _mountPoint;
            }

            // 实例化
            _instance = Object.Instantiate(prefab, parent);

            // 设置变换
            ApplyTransform();

            // 设置Layer
            ApplyLayer();

            // 设置SortingLayer
            ApplySortingLayer();

            // 获取粒子系统
            _particleSystems = _instance.GetComponentsInChildren<ParticleSystem>(true);

            // 自动播放粒子
            if (Parameter.AutoPlayParticle)
            {
                PlayParticleSystems();
            }

            // 设置销毁跟随
            if (Parameter.DestroyWithHost && _mountPoint != null)
            {
                // 通过检查宿主是否销毁来决定是否销毁实例
                // 实际销毁逻辑在OnRemove/OnDestroy中处理
            }
        }

        private void ApplyTransform()
        {
            if (_instance == null) return;

            var t = _instance.transform;

            if (Parameter.UseWorldSpace)
            {
                // 世界坐标模式
                if (_mountPoint != null)
                {
                    t.position = _mountPoint.position + Parameter.LocalPosition;
                    t.rotation = _mountPoint.rotation * Quaternion.Euler(Parameter.LocalRotation);
                }
                else
                {
                    t.position = Parameter.LocalPosition;
                    t.rotation = Quaternion.Euler(Parameter.LocalRotation);
                }
            }
            else
            {
                // 本地坐标模式
                t.localPosition = Parameter.LocalPosition;
                t.localRotation = Quaternion.Euler(Parameter.LocalRotation);
            }

            t.localScale = Parameter.LocalScale;
        }

        private void ApplyLayer()
        {
            if (_instance == null || Parameter.Layer < 0) return;

            if (Parameter.RecursiveLayer)
            {
                SetLayerRecursive(_instance.transform, Parameter.Layer);
            }
            else
            {
                _instance.layer = Parameter.Layer;
            }
        }

        private void SetLayerRecursive(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                SetLayerRecursive(child, layer);
            }
        }

        private void ApplySortingLayer()
        {
            if (_instance == null) return;

            var renderers = _instance.GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                if (Parameter.SortingOrder != 0)
                {
                    renderer.sortingOrder = Parameter.SortingOrder;
                }

                if (!string.IsNullOrEmpty(Parameter.SortingLayerName))
                {
                    renderer.sortingLayerName = Parameter.SortingLayerName;
                }
            }
        }

        #endregion

        #region 粒子系统控制

        private void PlayParticleSystems()
        {
            if (_particleSystems == null) return;

            foreach (var ps in _particleSystems)
            {
                if (ps != null)
                {
                    ps.Play();
                }
            }
        }

        private void StopParticleSystems()
        {
            if (_particleSystems == null) return;

            foreach (var ps in _particleSystems)
            {
                if (ps != null && ps.isPlaying)
                {
                    var stopBehavior = ConvertStopAction(Parameter.ParticleStopAction);
                    ps.Stop(true, stopBehavior);
                }
            }
        }

        private static ParticleSystemStopBehavior ConvertStopAction(ParticleSystemStopAction action)
        {
            return action switch
            {
                ParticleSystemStopAction.None => ParticleSystemStopBehavior.StopEmitting,
                ParticleSystemStopAction.Disable => ParticleSystemStopBehavior.StopEmittingAndClear,
                ParticleSystemStopAction.Destroy => ParticleSystemStopBehavior.StopEmittingAndClear,
                _ => ParticleSystemStopBehavior.StopEmitting
            };
        }

        #endregion

        #region 销毁逻辑

        private void ScheduleDestroy(float time)
        {
            _destroyStartTime = time;
            _pendingDestroy = true;

            if (Parameter.DestroyDelay <= 0f)
            {
                DestroyInstance();
            }
        }

        private void DestroyInstance()
        {
            if (_isDestroying) return;
            _isDestroying = true;
            _pendingDestroy = false;

            if (_instance != null)
            {
                // 根据粒子停止行为处理
                if (Parameter.ParticleStopAction == ParticleSystemStopAction.Destroy)
                {
                    // 让粒子自然播放完毕后销毁
                    if (_particleSystems != null && _particleSystems.Length > 0)
                    {
                        foreach (var ps in _particleSystems)
                        {
                            if (ps != null)
                            {
                                var main = ps.main;
                                main.stopAction = ParticleSystemStopAction.Destroy;
                            }
                        }
                        _instance = null;
                        return;
                    }
                }

                Object.Destroy(_instance);
                _instance = null;
            }

            _particleSystems = null;
            _mountPoint = null;
            _isDestroying = false;
        }

        #endregion

        #region 公开接口

        /// <summary>
        /// 获取实例化的GameObject
        /// </summary>
        public GameObject Instance => _instance;

        /// <summary>
        /// 获取挂载点Transform
        /// </summary>
        public Transform MountPoint => _mountPoint;

        /// <summary>
        /// 手动设置实例的位置
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            if (_instance == null) return;

            if (Parameter.UseWorldSpace)
            {
                _instance.transform.position = position;
            }
            else
            {
                _instance.transform.localPosition = position;
            }
        }

        /// <summary>
        /// 手动设置实例的旋转
        /// </summary>
        public void SetRotation(Quaternion rotation)
        {
            if (_instance == null) return;

            if (Parameter.UseWorldSpace)
            {
                _instance.transform.rotation = rotation;
            }
            else
            {
                _instance.transform.localRotation = rotation;
            }
        }

        /// <summary>
        /// 手动设置实例的缩放
        /// </summary>
        public void SetScale(Vector3 scale)
        {
            if (_instance == null) return;
            _instance.transform.localScale = scale;
        }

        /// <summary>
        /// 手动播放粒子系统
        /// </summary>
        public void PlayParticles()
        {
            PlayParticleSystems();
        }

        /// <summary>
        /// 手动停止粒子系统
        /// </summary>
        public void StopParticles()
        {
            StopParticleSystems();
        }

        #endregion

        #region 编辑器预览

#if UNITY_EDITOR
        public override void OnPreview(GameObject target, int frame, int startFrame, int endFrame)
        {
            base.OnPreview(target, frame, startFrame, endFrame);

            if (target == null || string.IsNullOrEmpty(Parameter.PrefabPath)) return;

            if (frame == startFrame)
            {
                // 查找挂载点
                Transform mountPoint;
                if (string.IsNullOrEmpty(Parameter.MountPointPath))
                {
                    mountPoint = target.transform;
                }
                else
                {
                    mountPoint = target.transform.Find(Parameter.MountPointPath);
                    if (mountPoint == null) mountPoint = target.transform;
                }

                // 加载Prefab
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Parameter.PrefabPath);
                if (prefab == null) return;

                // 实例化预览
                var previewInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                if (previewInstance == null) return;

                // 设置父节点
                if (Parameter.FollowHost)
                {
                    previewInstance.transform.SetParent(mountPoint);
                }

                // 设置变换
                if (Parameter.UseWorldSpace)
                {
                    previewInstance.transform.position = mountPoint.position + Parameter.LocalPosition;
                    previewInstance.transform.rotation = mountPoint.rotation * Quaternion.Euler(Parameter.LocalRotation);
                }
                else
                {
                    previewInstance.transform.localPosition = Parameter.LocalPosition;
                    previewInstance.transform.localRotation = Quaternion.Euler(Parameter.LocalRotation);
                }
                previewInstance.transform.localScale = Parameter.LocalScale;

                // 设置Layer
                if (Parameter.Layer >= 0)
                {
                    if (Parameter.RecursiveLayer)
                    {
                        SetLayerRecursive(previewInstance.transform, Parameter.Layer);
                    }
                    else
                    {
                        previewInstance.layer = Parameter.Layer;
                    }
                }

                // 标记为场景对象用于预览
                previewInstance.name = $"[Preview] {prefab.name}";
            }
        }
#endif

        #endregion
    }
}