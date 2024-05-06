using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;

namespace GAS.Runtime
{
    public class AbilityContainer
    {
        private readonly AbilitySystemComponent _owner;
        private readonly Dictionary<string, AbilitySpec> _abilities = new Dictionary<string, AbilitySpec>();
        private readonly List<AbilitySpec> _cachedAbilities = new List<AbilitySpec>();

        public AbilityContainer(AbilitySystemComponent owner)
        {
            _owner = owner;
        }

        public void Tick()
        {
            Profiler.BeginSample($"{nameof(AbilityContainer)}::Tick()");

            _cachedAbilities.AddRange(_abilities.Values);

            foreach (var abilitySpec in _cachedAbilities)
            {
                Profiler.BeginSample("abilitySpec.Tick()");
                abilitySpec.Tick();
                Profiler.EndSample();
            }

            _cachedAbilities.Clear();

            Profiler.EndSample();
        }

        public void GrantAbility(AbstractAbility ability)
        {
            if (_abilities.ContainsKey(ability.Name)) return;
            var abilitySpec = ability.CreateSpec(_owner);
            _abilities.Add(ability.Name, abilitySpec);
        }

        public void RemoveAbility(AbstractAbility ability)
        {
            RemoveAbility(ability.Name);
        }

        public void RemoveAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;

            EndAbility(abilityName);
            _abilities.Remove(abilityName);
        }

        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            if (!_abilities.ContainsKey(abilityName)) return false;
            if (!_abilities[abilityName].TryActivateAbility(args)) return false;

            var tags = _abilities[abilityName].Ability.Tag.CancelAbilitiesWithTags;
            foreach (var kv in _abilities)
            {
                var abilityTag = kv.Value.Ability.Tag;
                if (abilityTag.AssetTag.HasAnyTags(tags))
                {
                    _abilities[kv.Key].TryCancelAbility();
                }
            }

            return true;
        }

        public void EndAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].TryEndAbility();
        }

        public void CancelAbility(string abilityName)
        {
            if (!_abilities.ContainsKey(abilityName)) return;
            _abilities[abilityName].TryCancelAbility();
        }

        void CancelAbilitiesByTag(GameplayTagSet tags)
        {
            foreach (var kv in _abilities)
            {
                var abilityTag = kv.Value.Ability.Tag;
                if (abilityTag.AssetTag.HasAnyTags(tags))
                {
                    _abilities[kv.Key].TryCancelAbility();
                }
            }
        }

        public Dictionary<string, AbilitySpec> AbilitySpecs() => _abilities;

        public void CancelAllAbilities()
        {
            foreach (var kv in _abilities)
                _abilities[kv.Key].TryCancelAbility();
        }

        public bool HasAbility(string abilityName) => _abilities.ContainsKey(abilityName);

        public bool HasAbility(AbstractAbility ability) => HasAbility(ability.Name);
    }
}