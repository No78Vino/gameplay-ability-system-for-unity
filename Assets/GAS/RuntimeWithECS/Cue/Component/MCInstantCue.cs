using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class MCInstantCue : IComponentData
    {
        public CueInstant cue;
        
        public MCInstantCue()
        {
        }
        
        public MCInstantCue(CueInstant cue)
        {
            this.cue = cue;
        }
    }
}