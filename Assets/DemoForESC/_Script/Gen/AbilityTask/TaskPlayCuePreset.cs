namespace GAS.Runtime
{
    public class TaskPlayCuePreset: AbilityTaskBase<XParamCueList>
    {
        private GameplayCueUnit[] _cueUnits;
        
        public TaskPlayCuePreset(AbilityLogicBase logic) : base(logic)
        {
        }

        public override void InitParameters(IExParameterBase parameter)
        {
            base.InitParameters(parameter);
            _cueUnits = new GameplayCueUnit[Parameter.Cues.Length];
            for (var i = 0; i < Parameter.Cues.Length; i++)
            {
                var cueID = Parameter.Cues[i];
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

        public override void OnEditorPreview(int frame, int startFrame, int endFrame)
        {
            base.OnEditorPreview(frame, startFrame, endFrame);
            foreach (var cueUnit in _cueUnits)
            {
                cueUnit.OnPreview(frame, startFrame, endFrame);
            }
        }
    }
}