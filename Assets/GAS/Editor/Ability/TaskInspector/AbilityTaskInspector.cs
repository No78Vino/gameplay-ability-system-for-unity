#if UNITY_EDITOR
namespace GAS.Editor
{
    using GAS.Runtime.Ability;
    using UnityEngine.UIElements;

    public abstract class AbilityTaskInspector
    {
        protected AbilityTaskBase _taskBase;

        public AbilityTaskInspector(AbilityTaskBase taskBase)
        {
            _taskBase = taskBase;
        }

        public abstract VisualElement Inspector();

        protected abstract void Save();
    }
}
#endif