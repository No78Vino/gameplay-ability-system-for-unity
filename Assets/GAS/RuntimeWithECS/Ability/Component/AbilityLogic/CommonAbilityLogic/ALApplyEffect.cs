using GAS.Runtime;
using GAS.RuntimeWithECS.GameplayEffect;
using GAS.RuntimeWithECS.Static;
using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.CommonAbilityLogic
{
    public class ALApplyEffect : AbilityLogicBase<AbilityParamArrayInt>
    {
        public ALApplyEffect(Entity ability) : base(ability)
        {
        }

        public void InitGameplayEffects(int[] effects)
        {
            _param.SetValue(effects);
        }

        public override void AbilityTick(GlobalTimer timer)
        {
        }

        public override void ActivateAbility(GlobalTimer timer)
        {
            var baseInfo = _entityManager.GetComponentData<CAbilityBaseInfo>(_abilityEntity);
            var owner = baseInfo.Owner;
            foreach (var effectPath in _param.Value)
            {
                // TODO
                // string resourcePath = GetResourcePath(effectPath);
                // var effect = Resources.Load<GameplayEffectConfigAsset>(resourcePath);
                // if (effect == null) continue;
                // var geEntity = CreateGameplayEffectEntity(effect.GetConfig());
                // ApplyGameplayEffectTo(geEntity, owner, owner);
            }
        }

        public override void CancelAbility(GlobalTimer timer)
        {
            EndAbility(timer);
        }

        public override void EndAbility(GlobalTimer timer)
        {
            var ownerAsc = GetOwnerAsc();
            var geEntities = _entityManager.GetBuffer<BEGameplayEffect>(ownerAsc);
            foreach (var beEffect in geEntities)
            {
                var effect = beEffect.GameplayEffect;
                if (_entityManager.HasComponent<CCreatedByAbility>(effect))
                {
                    var createdByAbility = _entityManager.GetComponentData<CCreatedByAbility>(effect);
                    if (createdByAbility.sourceAbility == _abilityEntity)
                        RemoveGameplayEffect(effect);
                }
            }
        }

        private string GetResourcePath(string effectPath)
        {
            // 只截取Resources的子路径：找到路径里匹配"Resources/"的位置，然后只保留后面的路径。
            var index = effectPath.IndexOf("Resources/");
            if (index >= 0)
            {
                var p = effectPath.Substring(index + "Resources/".Length);
                // 再删掉“.asset”后缀
                if (p.EndsWith(".asset"))
                {
                    p = p.Substring(0, p.Length - ".asset".Length);
                }

                return p;
            }

            Debug.LogWarning($"Effect path '{effectPath}' does not contain 'Resources/'. Returning the original path.");
            return effectPath; // 如果没有找到，返回原始路径
        }
    }
}