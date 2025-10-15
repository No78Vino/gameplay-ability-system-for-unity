namespace GAS.Runtime
{
    public interface IGameplayEffectData
    {
        string GetDisplayName();
        EffectsDurationPolicy GetDurationPolicy();
        float GetDuration();
        float GetPeriod();

        /// <summary>
        /// 必须是Instant型的GameplayEffect
        /// </summary>
        IGameplayEffectData GetPeriodExecution();

        GameplayTag[] GetAssetTags();
        GameplayTag[] GetGrantedTags();
        GameplayTag[] GetRemoveGameplayEffectsWithTags();
        GameplayTag[] GetApplicationRequiredTags();
        GameplayTag[] GetApplicationImmunityTags();
        GameplayTag[] GetOngoingRequiredTags();

        // Modifiers
        GameplayEffectModifier[] GetModifiers();

        // Granted Ability
        GrantedAbilityConfig[] GetGrantedAbilities();
        
        //Stacking
        GameplayEffectStacking GetStacking();
    }
}