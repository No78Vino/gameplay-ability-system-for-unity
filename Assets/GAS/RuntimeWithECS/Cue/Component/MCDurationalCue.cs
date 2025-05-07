using Unity.Entities;

namespace GAS.RuntimeWithECS.Cue.Component
{
    public class MCDurationalCue : IComponentData
    {
        public CueDurational cue;
        
        public MCDurationalCue()
        {
        }
        
        public MCDurationalCue(CueDurational cue)
        {
            this.cue = cue;
        }
    }
}