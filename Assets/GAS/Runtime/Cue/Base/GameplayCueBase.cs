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

        protected static EntityManager EntityManager => GASManager.EntityManager; 
        
        public abstract void InitParameters(ICueParameter parameter);
        public abstract void Reset();
        
        public void SetCueEntity(Entity e)
        {
            _cueEntity = e;
        }

        public void SetSourceEntity(Entity e,CueSourceType sourceType)
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
                OnAdd(Time.time);
            }
        }
        
        /// <summary>
        /// cue从目标ASC移除
        /// </summary>
        public void RemoveFromTargetAsc()
        {
            OnRemove(Time.time);
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
                EntityHelper.SetComponentEnabled<ECCuePlayable>(_cueEntity,true);
                if (replay)
                {
                    Reset();
                    EntityHelper.SetComponentEnabled<ECCuePlaying>(_cueEntity,false);
                }
            }
        }

        /// <summary>
        /// 停止Cue
        /// </summary>
        /// <param name="immediate"> 是否立即停止 </param>
        public void Stop(bool immediate = false)
        {
            EntityManager.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
        }
        
        public void StopImmediate() => Stop(true);

        public void KillSelf()
        {
            EntityManager.SetComponentEnabled<ECKillCue>(_cueEntity,true);
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
        #endregion
    }

    public abstract class GameplayCueBase<T> : GameplayCueBase where T : ICueParameter
    {
        public T Parameter { get; private set; }
        
        public override void InitParameters(ICueParameter parameter)
        {
            if (parameter is T t)
                Parameter = t;
#if UNITY_EDITOR
            else
                Debug.LogError($"Parameter type mismatch: expected {typeof(T)}, but got {parameter.GetType()}");
#endif
        }
    }
}