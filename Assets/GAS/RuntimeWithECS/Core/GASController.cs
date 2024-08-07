using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    public class GASController : MonoBehaviour
    {
        public bool isGASRunning;
        
        private class GASContorllerBaker : Baker<GASController>
        {
            public override void Bake(GASController authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                if (authoring.isGASRunning) AddComponent<GASRunningTagComponent>(entity);
            }
        }
    }
    
    public struct GASRunningTagComponent : IComponentData
    {
        
    }
}