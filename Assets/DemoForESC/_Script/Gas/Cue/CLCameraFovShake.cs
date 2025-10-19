using GAS.Runtime;

namespace DemoForESC._Script.Gas.Cue
{
    public class CLCameraFovShake:GameplayCueBase<CueParamFloat>
    {
        public override void OnActivate(float time)
        {
            base.OnActivate(time);
            
            RemoveSelf();
        }
        
        public override void Reset()
        {
        }
    }
}