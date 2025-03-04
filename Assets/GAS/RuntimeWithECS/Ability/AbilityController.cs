using System.Collections.Generic;
using GAS.RuntimeWithECS.Ability.Component.Dynamic;
using GAS.RuntimeWithECS.Ability.Component.Static;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability
{
    /// <summary>
    /// Ability controller
    /// 能力控制器，对应旧Mono版本AbilityContainer
    /// </summary>
    public class AbilityController
    {
        private readonly Entity _asc;
        private static EntityManager GasEntityManager => GASManager.EntityManager;
        
        public DynamicBuffer<BEAbility> CurrentAbilities =>
            GasEntityManager.GetBuffer<BEAbility>(_asc);
        
        public AbilityController(Entity asc)
        {
            _asc = asc;
            GasEntityManager.AddBuffer<BEAbility>(_asc);
        }
        
        public void GrantAbility(Entity ability)
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            buffer.Add(new BEAbility {Ability = ability});
        }
        
        public void RemoveAbility(int abilityCode)
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                {
                    buffer.RemoveAt(i);
                    break;
                }
            }
        }
        
        public void RemoveAbility(Entity ability)
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].Ability == ability)
                {
                    buffer.RemoveAt(i);
                    break;
                }
            }
        }
        
        public void ClearAbilities()
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            buffer.Clear();
        }
        
        public bool HasAbility(int abilityCode)
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                {
                    return true;
                }
            }
            return false;
        }
        
        public void TryActivateAbility(int abilityCode, params object[] args)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                {
                    GasEntityManager.AddComponent<CAbilityInTryActivate>(a);
                    break;
                }
            }
        }
        
        public void EndAbility(int abilityCode)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                {
                    // TODO
                    // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                    // ability.EndAbility();
                    break;
                }
            }
        }
        
        public void EndAllAbilities()
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                // TODO
                // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                // ability.EndAbility();
            }
        }
        
        public void CancelAbility(int abilityCode)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                {
                    // TODO
                    // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                    // ability.CancelAbility();
                    break;
                }
            }
        }
        
        public void CancelAllAbilities()
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                // TODO
                // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                // ability.CancelAbility();
            }
        }
        
        public void CancelAbilitiesByTags(params int[] tags)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                // TODO
                // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                // if (ability.Tag.HasAnyTags(tags))
                // {
                //     ability.CancelAbility();
                // }
            }
        }
        
        public void CancelAbilitiesByTags(List<int> tags)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                // TODO
                // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                // if (ability.Tag.HasAnyTags(tags))
                // {
                //     ability.CancelAbility();
                // }
            }
        }
    }
}