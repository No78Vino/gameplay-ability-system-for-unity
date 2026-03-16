using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public abstract class GameplayCueBase
    {
        protected Entity _cueEntity;
        protected Entity _sourceEntity;
        protected CueSourceType _sourceType;
        protected Entity _targetAscEntity;
        protected AbilitySystemCell _abilitySystemCell;
        protected static EntityManager EntityManager => GASManager.EntityManager;

        protected GameplayEffectSpec _effectSpec;
        
        public abstract void InitParameters(XParam xParam);

        public virtual void Reset()
        {
        }

        public void SetCueEntity(Entity e)
        {
            _cueEntity = e;
        }

        public void SetSourceEntity(Entity e, CueSourceType sourceType)
        {
            _sourceEntity = e;
            _sourceType = sourceType;
        }

        /// <summary>
        /// 添加Cue到目标ASC
        /// </summary>
        /// <param name="e"></param>
        public void AddToTargetAsc(Entity e)
        {
            if (e != Entity.Null)
            {
                _targetAscEntity = e;
                _abilitySystemCell = GASManager.GetAscFromEntity(_targetAscEntity);
                OnAdd(Time.time);
            }
        }

        /// <summary>
        /// cue从目标ASC移除
        /// </summary>
        public void RemoveFromTargetAsc()
        {
            OnRemove(Time.time);
            _abilitySystemCell = null;
            _targetAscEntity = Entity.Null;
        }

        /// <summary>
        /// 自定义能否播放cue逻辑
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanPlay()
        {
            return true;
        }

        /// <summary>
        /// 播放Cue
        /// </summary>
        /// <param name="replay"> 是否从头播放 </param>
        public void Play(bool replay = false)
        {
            if (CanPlay())
            {
                EntityHelper.SetComponentEnabled<ECCuePlayable>(_cueEntity, true);
                if (replay)
                {
                    Reset();
                    EntityHelper.SetComponentEnabled<ECCuePlaying>(_cueEntity, false);
                }
            }
        }

        /// <summary>
        /// 停止Cue
        /// </summary>
        /// <param name="immediate"> 是否立即停止 </param>
        public void Stop(bool immediate = false)
        {
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity, false);
        }

        public void StopImmediate() => Stop(true);

        public void KillSelf()
        {
            EntityManager.SetComponentEnabled<ECKillCue>(_cueEntity, true);
        }

        public void RemoveSelf()
        {
            StopImmediate();
            RemoveFromTargetAsc();
        }

        public GameplayEffectSpec GetEffectSpec()
        {
            if (_sourceType != CueSourceType.GameplayEffect) return null;

            if (_effectSpec != null)
            {
                if (_effectSpec.IsValid) return _effectSpec;

                _sourceEntity = Entity.Null;
                _effectSpec = null;
                return null;
            }

            if (_sourceEntity == Entity.Null || !EntityManager.Exists(_sourceEntity)) return null;
            _effectSpec = new GameplayEffectSpec(_sourceEntity);
            return _effectSpec;

        }

        #region system function

        public virtual void OnAdd(float time)
        {
        }

        public virtual void OnRemove(float time)
        {
        }

        public virtual void OnActivate(float time)
        {
        }

        public virtual void OnDeactivate(float time)
        {
        }

        public virtual void OnTick(float time)
        {
        }

        public virtual void OnDestroy(float time)
        {
        }
        
#if UNITY_EDITOR
        /// <summary>
        ///     编辑器预览Cue效果
        ///     注意：该方法只在编辑器下有效，运行时无效。
        ///     请使用 UNITY_EDITOR 宏来包裹该方法，否则在运行时会导致编译错误。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public virtual void OnPreview(GameObject target, int frame, int startFrame, int endFrame)
        {

        }
#endif

        #endregion
    }

    public abstract class GameplayCueBase<T> : GameplayCueBase where T : XParam
    {
        public T Parameter { get; private set; }
        
        public override void InitParameters(XParam xParam)
        {
            if (xParam is T t)
                Parameter = t;
#if UNITY_EDITOR
            else
                Debug.LogError($"Parameter type mismatch: expected {typeof(T)}, but got {xParam.GetType()}");
#endif
        }
    }
}