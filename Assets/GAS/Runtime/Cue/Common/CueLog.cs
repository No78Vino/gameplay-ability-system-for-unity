using UnityEngine;

namespace GAS.Runtime
{
    public class CueLog : GameplayCueBase<XParamString>
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

        public override void OnPreview(int frame, int startFrame, int endFrame,params object[] args)
        {
            base.OnPreview(frame, startFrame, endFrame);
            Debug.Log($"[Preview Frame {frame}]Msg:{Parameter.Value}");
        }
    }
}