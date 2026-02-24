using UnityEngine;

namespace GAS.Runtime
{
    public class TaskPlayCue : AbilityTaskBase<XParamCue>
    {
        private GameplayCueUnit _cueUnit;

        public TaskPlayCue(AbilityLogicBase logic) : base(logic)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            // _cueUnit.Destroy();
        }

        public override void InitParameters(XParam parameter)
        {
            base.InitParameters(parameter);
            _cueUnit = new GameplayCueUnit(Parameter.GetCueConfig());
        }

        protected override void OnBegin(int startFrame)
        {
            _cueUnit.Create();
            _cueUnit.AddToAsc(Owner);
            _cueUnit.Play();
        }

        protected override void OnFinish(int endFrame)
        {
            _cueUnit.Stop();
            _cueUnit.RemoveFromAsc();
            _cueUnit.Destroy();
        }

        protected override void OnTick(int frameIndex)
        {
            base.OnTick(frameIndex);
        }
        
#if UNITY_EDITOR
        public override void OnEditorPreview(GameObject target,int frame, int startFrame, int endFrame)
        {
            base.OnEditorPreview(target,frame, startFrame, endFrame);
            _cueUnit.OnPreview(target,frame, startFrame, endFrame);
        }
#endif
    }
}