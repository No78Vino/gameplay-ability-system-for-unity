using UnityEngine;

namespace GAS.Runtime
{
    public abstract class AbilityTaskBase
    {
        protected AbilityLogicBase _logic;
        protected AbilitySpec Spec => _logic.Spec;
        protected AbilitySystemCell Owner => _logic.Owner;
        protected TimeUnit _timeUnit = TimeUnit.Frame;
        protected int _startTime;

        public AbilityTaskBase()
        {
        }

        public virtual void Dispose()
        {
        }
        
        public AbilityTaskBase(AbilityLogicBase logic)
        {
            _logic = logic;
        }

        public abstract void InitParameters(XParam parameter);

        /// <summary>
        /// 修改 AbilityTask 的生效时间单位，默认是Frame（以帧为单位）
        /// </summary>
        /// <param name="timeUnit"></param>
        public void SetTimeUnit(TimeUnit timeUnit) => _timeUnit = timeUnit;

        /// <summary>
        /// 编辑器预览用
        /// 【注意】 覆写时，记得用UNITY_EDITOR宏包裹，这是预览表现用的函数，不该被编译。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public virtual void OnEditorPreview(GameObject target,int frame, int startFrame, int endFrame)
        {
        }
        
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

    public abstract class AbilityTaskBase<T> : AbilityTaskBase where T : XParam
    {
        public T Parameter { get; private set; }

        protected AbilityTaskBase(AbilityLogicBase logic) : base(logic)
        {
        }

        public override void InitParameters(XParam parameter)
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