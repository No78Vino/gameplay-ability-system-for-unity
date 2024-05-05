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
            if (sourceEffectSpec.Owner.AbilityContainer.HasAbility(Ability)) return null;
            
            return new GrantedAbilitySpecFromEffect(this,sourceEffectSpec);
        } 
    }
    
    public class GrantedAbilitySpecFromEffect
    {
        public readonly GrantedAbilityFromEffect GrantedAbility;
        public readonly GameplayEffectSpec SourceEffectSpec;
        public readonly string AbilityName;
        public readonly AbilitySystemComponent Owner;
        
        public bool Grabbed { get; private set; }
        
        public GrantedAbilitySpecFromEffect(GrantedAbilityFromEffect grantedAbility,GameplayEffectSpec sourceEffectSpec)
        {
            GrantedAbility = grantedAbility;
            SourceEffectSpec = sourceEffectSpec;
            AbilityName = GrantedAbility.Ability.Name;
            Owner = SourceEffectSpec.Owner;
            Owner.GrantAbility(GrantedAbility.Ability);
            Grabbed = false;
        }

        public AbilitySpec AbilitySpec => Owner.AbilityContainer.AbilitySpecs()[AbilityName];
        
        public bool Passive => GrantedAbility.Passive;
        
        public void Activate()
        {
            AbilitySpec.TryActivateAbility();
        }

        public void Deactivate()
        {
            AbilitySpec.TryEndAbility();
        }
        
        public void Grab() => Grabbed = true;
        public void UnGrab() => Grabbed = false;
    }
}