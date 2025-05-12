using GAS.RuntimeWithECS.Cue;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public class CueLog : CueInstant<CueParamString>
    {
        protected override void Trigger()
        {
            Debug.Log(
                $"SourceType:{_sourceType.ToString()}, Entity:{_sourceEntity} ,Msg:{Parameter.Value}");
        }

        public void SetMessage(string message)
        {
            Parameter.SetValue(message);
        }

        public CueLog(Entity sourceEntity, CueSourceType sourceType, CueParamString parameter) : base(sourceEntity, sourceType, parameter)
        {
        }

        public override void Reset()
        {
            
        }
    }
}