using Unity.Collections;
using Unity.Entities;

namespace GAS.Runtime
{
    public struct CAbilityCooldown : IComponentData
    {
        public int Cooldown;
        
        /// <summary>
        ///  冷却GE的原型
        /// </summary>
        public Entity ProtoGameplayEffectCooldown;
        
        /// <summary>  
        ///  冷却Tag列表，从原型GE的GrantedTags拷贝而来  
        ///  用于Tag索引方式检查CD状态  
        /// </summary>  
        public NativeArray<int> CooldownTags;  
    }
    
    public sealed class ConfAbilityCooldown : AbilityComponentConfig  
    {  
        public int Cooldown;  
        public GameplayEffectComponentConfig[] CooldownComponentConfigs;  
  
        public override void LoadToGameplayAbilityEntity(Entity ability)  
        {  
            var protoGe = GameplayEffectHelper.CreateGameplayEffectEntity(CooldownComponentConfigs);  
  
            // 从原型GE的GrantedTags中提取CooldownTags  
            NativeArray<int> cooldownTags = default;  
            if (_entityManager.HasComponent<CEffectGrantedTags>(protoGe))  
            {  
                var grantedTags = _entityManager.GetComponentData<CEffectGrantedTags>(protoGe);  
                cooldownTags = new NativeArray<int>(grantedTags.tags.ToArray(), Allocator.Persistent);  
            }  
  
            _entityManager.AddComponentData(ability, new CAbilityCooldown  
            {  
                Cooldown = Cooldown,  
                ProtoGameplayEffectCooldown = protoGe,  
                CooldownTags = cooldownTags,  
            });  
        }  
    }
}