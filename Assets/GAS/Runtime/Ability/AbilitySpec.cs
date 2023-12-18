// using GAS.Runtime.Effects;
//
// namespace GAS.Runtime.Ability
// {
//     public struct AbilityCooldownTime
//     {
//         public float TimeRemaining;
//         public float TotalDuration;
//     }
//
//     public abstract class AbilitySpec
//     {
//         public AbstractAbility Ability;
//         
//         public AbilitySystemComponent.AbilitySystemComponent Owner { get; protected set; }
//
//         public float Level { get; private set; }
//
//         public bool IsActive { get;private set; }
//         
//         public int ActiveCount { get;private set; }
//
//
//         public AbilitySpec(AbstractAbility ability, AbilitySystemComponent.AbilitySystemComponent owner)
//         {
//             this.Ability = ability;
//             this.Owner = owner;
//         }
//         
//         public abstract bool StepAbility();
//         
//         public virtual bool CanActivateAbility()
//         {
//             return !IsActive
//                     && CheckGameplayTags()
//                     && CheckCost()
//                     && CheckCooldown().TimeRemaining <= 0;
//         }
//         
//         public abstract bool CheckGameplayTags();
//         
//         public virtual AbilityCooldownTime CheckCooldown()
//         {
//             if (Ability.Cooldown.Empty) return new AbilityCooldownTime();
//             var cooldownTags = Ability.Cooldown.GetGameplayEffectTags().GrantedTags;
//
//             return Owner.CheckCooldownForTags(cooldownTags);
//         }
//         
//         public virtual void ActivateAbility()
//         {
//             if(IsActive) return;
//             IsActive = true;
//             ActiveCount++;
//             Ability.ActivateAbility();
//         }
//         
//         public virtual void EndAbility()
//         {
//             if(!IsActive) return;
//             IsActive = false;
//             Ability.EndAbility();
//         }
//
//         private GameplayEffectSpec _CostCache;
//
//         private GameplayEffectSpec TryGetCostSpec()
//         {
//             if (_CostCache == null || _CostCache.Level != Level)
//             {
//                 _CostCache = Owner.CreateGameplayEffectSpec(Ability.Cost, Level);
//             }
//             return _CostCache;
//         }
//
//
//         public virtual bool CheckCost()
//         {
//             if (Ability.Cost.Empty) return true;
//             var costGe = TryGetCostSpec();
//             if (costGe == null) return false;
//             
//             if (costGe.GameplayEffect.DurationPolicy != EffectsDurationPolicy.Instant) return true;
//
//             for (var i = 0; i < costGe.GameplayEffect.Modifiers.Count; i++)
//             {
//                 var modifier = costGe.GameplayEffect.Modifiers[i];
//
//                 // Only worry about additive.  Anything else passes.
//                 if (modifier.ModifierOperator != EAttributeModifier.Add) continue;
//                 var costValue = (modifier.ModifierMagnitude.CalculateMagnitude(costGe, modifier.Multiplier)).GetValueOrDefault();
//
//                 this.Owner.AttributeSystem.GetAttributeValue(modifier.Attribute, out var attributeValue);
//
//                 // The total attribute after accounting for cost should be >= 0 for the cost check to succeed
//                 if (attributeValue.CurrentValue + costValue < 0) return false;
//
//             }
//             return true;
//         }
//         
//         protected virtual bool AscHasAllTags(AbilitySystemComponent.AbilitySystemComponent asc, GameplayTagScriptableObject.GameplayTag[] tags)
//         {
//             if (!asc) return true;
//
//             for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
//             {
//                 var abilityTag = tags[iAbilityTag];
//
//                 bool requirementPassed = false;
//                 for (var iAscTag = 0; iAscTag < asc.AppliedTags.Count; iAscTag++)
//                 {
//                     if (asc.AppliedTags[iAscTag].TagData == abilityTag)
//                     {
//                         requirementPassed = true;
//                         continue;
//                     }
//                 }
//                 // If any ability tag wasn't found, requirements failed
//                 if (!requirementPassed) return false;
//             }
//             return true;
//         }
//
//         protected virtual bool AscHasNoneTags(AbilitySystemCharacter asc, GameplayTagScriptableObject.GameplayTag[] tags)
//         {
//             // If the input ASC is not valid, assume check passed
//             if (!asc) return true;
//
//             for (var iAbilityTag = 0; iAbilityTag < tags.Length; iAbilityTag++)
//             {
//                 var abilityTag = tags[iAbilityTag];
//
//                 bool requirementPassed = true;
//                 for (var iAscTag = 0; iAscTag < asc.AppliedTags.Count; iAscTag++)
//                 {
//                     if (asc.AppliedTags[iAscTag].TagData == abilityTag)
//                     {
//                         requirementPassed = false;
//                     }
//                 }
//                 // If any ability tag wasn't found, requirements failed
//                 if (!requirementPassed) return false;
//             }
//             return true;
//         }
//     }
//
// }
// }