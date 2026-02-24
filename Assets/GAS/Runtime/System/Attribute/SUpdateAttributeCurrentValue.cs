using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGAttribute))]
    public partial struct SUpdateAttributeCurrentValue : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BEAttrSet>();
            state.RequireForUpdate<CAttributeIsDirty>();
        }

        //[BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = EntityHelper.RegisterEntityCommandBuffer();
            
            foreach (var (_,attrSets,asc) in SystemAPI.Query<RefRO<CAttributeIsDirty>,DynamicBuffer<BEAttrSet>>().WithEntityAccess())
            {
                foreach (var attrSet in attrSets)
                {
                    foreach (var attr in attrSet.Attributes)
                    {
                        if (attr.Dirty)
                        {
                            AttributeHelper.RecalculateCurrentValue(asc, attrSet.Code, attr.Code);
                        }
                    }
                }

                ecb.RemoveComponent<CAttributeIsDirty>(asc);
            }
            
            ecb.Playback(state.EntityManager);
            EntityHelper.ExecuteDeferredCommands();
            ecb.Dispose();
            EntityHelper.UnregisterEntityCommandBuffer();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
    }
}