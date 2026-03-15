using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CCueOnTick : IComponentData  
    {  
        public NativeArray<Entity> cues;  
      
        /// <summary>  
        /// 运行时的实际Cue实例  
        /// </summary>  
        public NativeArray<Entity> runtimeCues;  
    }

    public sealed class ConfCueOnTick : ConfCueBase
    {
        public override void LoadToGameplayEffectEntity(Entity ge)
        {
            var entities = CreateCueEntityArray(ge);
            EntityHelper.AddComponent<CCueOnTick>(ge);
            EntityHelper.SetComponent(ge, new CCueOnTick
            {
                cues = entities
            });
        }
    }
}