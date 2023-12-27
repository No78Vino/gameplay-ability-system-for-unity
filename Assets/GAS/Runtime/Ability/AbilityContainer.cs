using System.Collections.Generic;
using GAS.Runtime.Component;

namespace GAS.Runtime.Ability
{
    public class AbilityContainer
    {
        AbilitySystemComponent _owner;
        private readonly Dictionary<string, AbilitySpec> _abilities = new Dictionary<string, AbilitySpec>();
        
        public AbilityContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }
        
        public void Tick()
        {
            foreach (var abilitySpec in _abilities.Values)
            {
                abilitySpec.Tick();
            }
        }
        
        public void GrantAbility(AbstractAbility ability)
        {
            if (_abilities.ContainsKey(ability.Name)) return;
            var abilitySpec = ability.CreateSpec(_owner);
            _abilities.Add(ability.Name, abilitySpec);
            
            _owner.AddTags(abilitySpec.Ability.tag.ActivationOwnedTag);
        }
        
        public void RemoveAbility(AbstractAbility ability)
        {
            RemoveAbility(ability.Name);
        }
        
        public void RemoveAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            
            EndAbility(abilityName);
            _owner.RemoveTags(_abilities[abilityName].Ability.tag.ActivationOwnedTag);
            _abilities.Remove(abilityName);
        }
        
        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            if (!_abilities.ContainsKey(abilityName)) return false;
            _abilities[abilityName].TryActivateAbility(args);
            return true;
        }
        
        public void EndAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].EndAbility();
        }
    }
}