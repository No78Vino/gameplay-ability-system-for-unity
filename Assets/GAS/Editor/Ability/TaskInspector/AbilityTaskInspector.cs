#if UNITY_EDITOR
namespace GAS.Editor
{
    using Runtime;
    
    public class AbilityTaskInspector
    {
        protected AbilityTaskBase _task;
        public virtual void Init(AbilityTaskBase task)
        {
            _task = task;
        }
    }
}

#endif