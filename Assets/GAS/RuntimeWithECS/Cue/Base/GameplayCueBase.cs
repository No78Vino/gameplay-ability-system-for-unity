using System.Linq;
using GAS.General;
using GAS.Runtime;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Cue
{
    public abstract class GameplayCueBase
    {
        protected const int WIDTH_LABEL = 70;
        
        protected Entity _cueEntity;
        protected Entity _sourceEntity;
        protected CueSourceType _sourceType;
        protected Entity _targetAscEntity;
        
        protected virtual bool Triggerable()
        {
            return true;
        }
        
        public void SetCueEntity(Entity e)
        {
            _cueEntity = e;
        }

        public void SetSourceEntity(Entity e)
        {
            _sourceEntity = e;
        }

        public void SetTargetAscEntity(Entity e)
        {
            _targetAscEntity = e;
        }

        public void SetSourceType(CueSourceType sourceType)
        {
            _sourceType = sourceType;
        }

        public abstract void InitParameters(ICueParameter parameter);

        public abstract void Reset();

        protected abstract void Trigger();

        public void StopPlaying()
        {
            GASManager.EntityManager.IsComponentEnabled<ECCuePlaying>(_cueEntity);
        }

        #region system function

        public bool TryTrigger()
        {
            var triggerable = Triggerable();
            if (triggerable) Trigger();
            return triggerable;
        }

        public virtual void OnAdd()
        {
        }

        public virtual void OnRemove()
        {
        }

        public virtual void OnGameplayEffectActivate()
        {
        }

        public virtual void OnGameplayEffectDeactivate()
        {
        }

        public virtual void OnTick()
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