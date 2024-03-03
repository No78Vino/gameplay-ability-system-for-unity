using GAS.Runtime.Ability.TargetCatcher;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public class CatchSelfInspector:TargetCatcherInspector<CatchSelf>
    {
        public CatchSelfInspector(CatchSelf targetCatcherBase) : base(targetCatcherBase)
        {
        }

        public override VisualElement Inspector()
        {
            return new VisualElement();
        }
    }
}