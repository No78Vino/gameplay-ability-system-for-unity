using System.Collections.Generic;
using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    ///     Ability controller
    ///     能力控制器，对应旧Mono版本AbilityContainer
    /// </summary>
    public class AbilityController
    {
        private readonly Entity _asc;

        private Dictionary<int, AbilitySpec> _specs = new Dictionary<int, AbilitySpec>();
        
        public AbilityController(Entity asc)
        {
            _asc = asc;
            EntityHelper.AddBuffer<BAbility>(_asc);
        }
        
        public DynamicBuffer<BAbility> CurrentAbilities =>
            EntityHelper.GetBuffer<BAbility>(_asc);

        /// <summary>
        /// 获取Mono【OOP】形态的能力实例
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AbilitySpec GetAbilitySpec(int code)
        {
            return _specs.GetValueOrDefault(code);
        }

        public Dictionary<int,AbilitySpec> GetAllAbilitySpecs()
        {
            return _specs;
        }

        public void GrantAbility(AbilityConfig abilityConfig)
        {
            var abilityEntity = AbilityHelper.CreateAbilityEntity(abilityConfig.ComponentConfigs);
            AttachAbility(abilityEntity);
        }

        private void AttachAbility(Entity ability)
        {
            // 设置ability的owner
            var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(ability);
            abi.Owner = _asc;
            EntityHelper.SetComponent(ability, abi);
            
            var buffer = EntityHelper.GetBuffer<BAbility>(_asc);
            buffer.Add(new BAbility { Ability = ability });
            
            _specs[abi.Code] = new AbilitySpec(ability);
        }

        public void RemoveAbility(int abilityCode)
        {
            var buffer = EntityHelper.GetBuffer<BAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                {
                    buffer.RemoveAt(i);
                    _specs.Remove(abilityCode);
                    break;
                }
            }
        }

        public void RemoveAbility(Entity ability)
        {
            var buffer = EntityHelper.GetBuffer<BAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
                if (buffer[i].Ability == ability)
                {
                    buffer.RemoveAt(i);
                    break;
                }
        }

        public void ClearAbilities()
        {
            var buffer = EntityHelper.GetBuffer<BAbility>(_asc);
            buffer.Clear();
        }

        private Entity GetAbilityEntity(int abilityCode)
        {
            var buffer = EntityHelper.GetBuffer<BAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode) return a;
            }

            return Entity.Null;
        }
        
        public bool HaveAbility(int abilityCode)
        {
            var buffer = EntityHelper.GetBuffer<BAbility>(_asc);
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
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
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode)
                    return EntityHelper.GetManagedComponentData<MCAbilityLogic>(a);
            }

            return default;
        }
        
        public bool IsAbilityActive(int abilityCode)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code == abilityCode) return EntityHelper.HasComponent<CAbilityActive>(a);
            }

            return false;
        }

        public void TryActivateAbility(int abilityCode, XParam param = null)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code != abilityCode) continue;
                var logic = EntityHelper.GetManagedComponentData<MCAbilityLogic>(a);
                logic.Logic.SetParam(param);
                EntityHelper.AddComponent<CAbilityInTryActivate>(a);
                break;
            }
        }

        public void SetAbilityParam(int abilityCode, XParam param)
        {
            var buffer = CurrentAbilities;
            for (var i = 0; i < buffer.Length; i++)
            {
                var a = buffer[i].Ability;
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code != abilityCode) continue;
                var logic = EntityHelper.GetManagedComponentData<MCAbilityLogic>(a);
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
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);
                if (abi.Code != abilityCode) continue;
                EntityHelper.AddComponent<CAbilityInTryEnd>(a);
                break;
            }
        }

        public void EndAllAbilities()  
        {  
            var buffer = CurrentAbilities;  
            for (var i = 0; i < buffer.Length; i++)  
            {  
                var a = buffer[i].Ability;  
                if (EntityHelper.HasComponent<CAbilityActive>(a))  
                    EntityHelper.AddComponent<CAbilityInTryEnd>(a);  
            }  
        }  

        public void CancelAbility(int abilityCode)  
        {  
            var buffer = CurrentAbilities;  
            for (var i = 0; i < buffer.Length; i++)  
            {  
                var a = buffer[i].Ability;  
                var abi = EntityHelper.GetComponentData<CAbilityBaseInfo>(a);  
                if (abi.Code == abilityCode)  
                {  
                    if (EntityHelper.HasComponent<CAbilityActive>(a))  
                        EntityHelper.AddComponent<CAbilityInTryCancel>(a);  
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
                if (EntityHelper.HasComponent<CAbilityActive>(a))  
                    EntityHelper.AddComponent<CAbilityInTryCancel>(a);  
            }  
        }  

        public void CancelAbilitiesByTags(params int[] tags)  
        {  
            var buffer = CurrentAbilities;  
            for (var i = 0; i < buffer.Length; i++)  
            {  
                var a = buffer[i].Ability;  
                if (!EntityHelper.HasComponent<CAbilityActive>(a)) continue;  
                if (!EntityHelper.HasComponent<CAbilityAssetTags>(a)) continue;  
          
                var assetTags = EntityHelper.GetComponentData<CAbilityAssetTags>(a);  
                bool match = false;  
                foreach (var tag in tags)  
                {  
                    foreach (var assetTag in assetTags.tags)  
                    {  
                        if (TagHelper.HasTag(assetTag, tag))  
                        {  
                            match = true;  
                            break;  
                        }  
                    }  
                    if (match) break;  
                }  
          
                if (match)  
                    EntityHelper.AddComponent<CAbilityInTryCancel>(a);  
            }  
        }  

        public void CancelAbilitiesByTags(List<int> tags)  
        {  
            CancelAbilitiesByTags(tags.ToArray());  
        }
    }
}