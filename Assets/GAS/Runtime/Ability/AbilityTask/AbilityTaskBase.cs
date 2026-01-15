using UnityEngine;

namespace GAS.Runtime
{
    public abstract class AbilityTaskBase
    {
        protected AbilityLogicBase _logic;
        protected AbilitySpec _spec;
        protected AbilitySystemCell _owner;
        protected TimeUnit _timeUnit = TimeUnit.Frame;
        protected int _startTime;
        
        public AbilityTaskBase(AbilityLogicBase logic)
        {
            _logic = logic;
            _spec = _logic.Spec;
            _owner = _logic.Owner;
        }
        
        public abstract void InitParameters(IExParameterBase parameter);

        /// <summary>
        /// 修改 AbilityTask 的生效时间单位，默认是Frame（以帧为单位）
        /// </summary>
        /// <param name="timeUnit"></param>
        public void SetTimeUnit(TimeUnit timeUnit) => _timeUnit = timeUnit;
#if UNITY_EDITOR
        /// <summary>
        /// 编辑器预览用
        /// 【注意】 覆写时，记得用UNITY_EDITOR宏包裹，这是预览表现用的函数，不该被编译。
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public virtual void OnEditorPreview(int frame, int startFrame, int endFrame)
        {
        }
#endif
        public void Begin(int startTime)
        {
            _startTime = startTime;
            OnBegin(startTime);
        }

        public void Tick(int tickTime)
        {
            OnTick(tickTime);
        }
        
        public void Finish(int endTime)
        {
            OnFinish(endTime);
        }
        
        protected virtual void OnBegin(int startFrame)
        {
        }

        protected virtual void OnFinish(int endFrame)
        {
        }

        protected virtual void OnTick(int frameIndex)
        {
        }
    }

    public abstract class AbilityTaskBase<T> : AbilityTaskBase where T : IExParameterBase
    {
        public T Parameter { get; private set; }

        protected AbilityTaskBase(AbilityLogicBase logic) : base(logic)
        {
        }
        
        public override void InitParameters(IExParameterBase parameter)
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