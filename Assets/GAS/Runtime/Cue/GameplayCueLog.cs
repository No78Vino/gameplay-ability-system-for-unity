using UnityEngine;

namespace GAS.Runtime
{
    public class GameplayCueLog : GameplayCueBase<XParamString>
    {
        public override void OnActivate(float time)
        {
            base.OnActivate(time);
            Debug.Log(
                $"[{time}]SourceType:{_sourceType.ToString()}, Entity:{_sourceEntity} ,Msg:{Parameter.Value}");
            
            StopImmediate();
            RemoveFromTargetAsc();
        }

        public void SetMessage(string message)
        {
            Parameter.SetValue(message);
        }

        public override void Reset()
        {
            
        }
    }
}