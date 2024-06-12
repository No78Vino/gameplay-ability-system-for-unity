using System.Collections.Generic;
using UnityEngine;

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
            _cachedAbilities.AddRange(_abilities.Values);

            foreach (var abilitySpec in _cachedAbilities)
            {
                abilitySpec.Tick();
            }

            _cachedAbilities.Clear();
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
            _abilities[abilityName].Dispose();
            _abilities.Remove(abilityName);
        }

        public bool TryActivateAbility(string abilityName, params object[] args)
        {
            if (!_abilities.ContainsKey(abilityName))
            {
                // 开发指南:
                // 如果你的Preset里配置了固有技能却没该技能(甚至_abilities里一个技能都没有)
                // 可能是你忘记调用ASC::Init(), 请检查AbilitySystemComponent的初始化
                // 通常我们使用ASC::InitWithPreset()来间接调用ASC::Init()执行初始化
#if UNITY_EDITOR
                // 这个输出可以删掉, 某些情况下确实会尝试激活不存在的技能(失败了也无所谓), 但是对开发期间的调试有帮助
                Debug.LogWarning(
                    $"you are trying to activate an ability that does not exist: " +
                    $"abilityName=\"{abilityName}\", GameObject=\"{_owner.name}\", " +
                    $"Preset={(_owner.Preset != null ? _owner.Preset.name : "null")}");
#endif
                return false;
            }

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