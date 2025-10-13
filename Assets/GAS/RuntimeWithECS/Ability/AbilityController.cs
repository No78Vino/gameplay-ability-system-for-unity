using System.Collections.Generic;
using GAS.Runtime;
using GAS.RuntimeWithECS.Dynamic;
using GAS.RuntimeWithECS.Static;
using Unity.Entities;

namespace GAS.RuntimeWithECS
{
    /// <summary>
    ///     Ability controller
    ///     能力控制器，对应旧Mono版本AbilityContainer
    /// </summary>
    public class AbilityController
    {
        private readonly Entity _asc;

        public AbilityController(Entity asc)
        {
            _asc = asc;
            GasEntityManager.AddBuffer<BEAbility>(_asc);
        }

        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public DynamicBuffer<BEAbility> CurrentAbilities =>
            GasEntityManager.GetBuffer<BEAbility>(_asc);

        public void GrantAbility(Entity ability)
        {
            // 设置ability的owner
            var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(ability);
            abi.Owner = _asc;
            GasEntityManager.SetComponentData(ability, abi);
            
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            buffer.Add(new BEAbility { Ability = ability });
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
                if (buffer[i].Ability == ability)
                {
                    buffer.RemoveAt(i);
                    break;
                }
        }

        public void ClearAbilities()
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            buffer.Clear();
        }

        public bool HaveAbility(int abilityCode)
        {
            var buffer = GasEntityManager.GetBuffer<BEAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode) return true;
            }

            return false;
        }

        public MCAbilityLogic GetAbilityLogic(int abilityCode)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                    return GasEntityManager.GetComponentData<MCAbilityLogic>(a);
            }

            return default;
        }
        
        public bool IsAbilityActive(int abilityCode)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode) return GasEntityManager.HasComponent<CAbilityActive>(a);
            }

            return false;
        }

        public void TryActivateAbility(int abilityCode, IAbilityParam param = null)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code != abilityCode) continue;
                GasEntityManager.AddComponent<CAbilityInTryActivate>(a);
                break;
            }
        }

        public void SetAbilityParam(int abilityCode, IAbilityParam param)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code != abilityCode) continue;
                var logic = GasEntityManager.GetComponentData<MCAbilityLogic>(a);
                logic.Logic.SetParam(param);
                break;
            }
        }
        public void EndAbility(int abilityCode)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = GasEntityManager.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code != abilityCode) continue;
                GasEntityManager.AddComponent<CAbilityInTryEnd>(a);
                break;
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
                    // TODO
                    // var ability = GasEntityManager.GetComponentObject<AbstractAbility>(a);
                    // ability.CancelAbility();
                    break;
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