namespace GAS.Runtime
{
    public class GrantedAbilityFromEffect
    {
        public readonly bool Passive;
        public readonly AbstractAbility Ability;
        public readonly GameplayEffect SourceEffect;
        
        public GrantedAbilityFromEffect(bool passive, AbstractAbility ability, GameplayEffect sourceEffect)
        {
            Passive = passive;
            Ability = ability;
            SourceEffect = sourceEffect;
        }
        
        public GrantedAbilitySpecFromEffect CreateSpec(GameplayEffectSpec sourceEffectSpec)
        {
            // 是否宿主已经持有该技能，如果已经持有，则不创建新的技能
            if (sourceEffectSpec.Owner.AbilityContainer(Ability))
            return new GrantedAbilitySpecFromEffect(this,sourceEffectSpec);
        } 
    }
    
    public class GrantedAbilitySpecFromEffect
    {
        public readonly GrantedAbilityFromEffect GrantedAbility;
        public readonly AbilitySpec AbilitySpec;
        public readonly GameplayEffectSpec SourceEffectSpec;
        
        public GrantedAbilitySpecFromEffect(GrantedAbilityFromEffect grantedAbility,GameplayEffectSpec sourceEffectSpec)
        {
            GrantedAbility = grantedAbility;
            SourceEffectSpec = sourceEffectSpec;
            AbilitySpec = grantedAbility.Ability.CreateSpec(SourceEffectSpec.Owner);
        }
        
        public bool Passive => GrantedAbility.Passive;
        
        public void Activate()
        {
            AbilitySpec.TryActivateAbility();
        }

        public void Deactivate()
        {
            AbilitySpec.TryEndAbility();
        }
    }
}