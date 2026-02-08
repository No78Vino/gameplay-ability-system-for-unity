using UnityEngine;

namespace GAS.Runtime
{
    public class TaskPlayCuePreset : AbilityTaskBase<XParamCueList>
    {
        private GameplayCueUnit[] _cueUnits;

        public TaskPlayCuePreset(AbilityLogicBase logic) : base(logic)
        {
        }

        public override void InitParameters(XParam parameter)
        {
            base.InitParameters(parameter);
            _cueUnits = new GameplayCueUnit[Parameter.IDs.Length];
            for (var i = 0; i < Parameter.IDs.Length; i++)
            {
                var cueID = Parameter.IDs[i];
                var cfg = XLuban.GetGameplayCueConfig(cueID);
                var unit = new GameplayCueUnit(cfg);
                _cueUnits[i] = unit;
            }
        }

        protected override void OnBegin(int startFrame)
        {
            foreach (var cueUnit in _cueUnits)
            {
                cueUnit.Create();
                cueUnit.AddToAsc(_owner);
                cueUnit.Play();
            }
        }

        protected override void OnFinish(int endFrame)
        {
            foreach (var cueUnit in _cueUnits)
            {
                cueUnit.Stop();
                cueUnit.RemoveFromAsc();
                cueUnit.Destroy();
            }
        }

        protected override void OnTick(int frameIndex)
        {
            base.OnTick(frameIndex);
        }
#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject target, int frame, int startFrame, int endFrame)
        {
            base.OnEditorPreview(target, frame, startFrame, endFrame);
            foreach (var cueUnit in _cueUnits)
            {
                cueUnit.OnPreview(target, frame, startFrame, endFrame);
            }
        }
#endif
    }
}