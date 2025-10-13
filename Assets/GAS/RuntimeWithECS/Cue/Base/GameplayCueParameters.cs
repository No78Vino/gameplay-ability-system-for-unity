using Unity.Entities;

namespace GAS.Runtime
{
    public enum CueSourceType
    {
        None,
        AbilitySystemCell,
        GameplayEffect
    }
    
    public class GameplayCueParametersBase
    {
        public CueSourceType SourceType;
        public Entity entity;
    }
}