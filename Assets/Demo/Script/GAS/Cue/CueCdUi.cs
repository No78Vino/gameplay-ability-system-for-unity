using GAS.Runtime.Cue;

namespace GAS.Cue
{
    public enum CdType
    {
        DodgeCd,
        FirBulletCd,
        PlayerBuff,
        BossBuff
    }
    
    public class CueCdUi:GameplayCueDurational
    {
        public CdType cdType;
        
        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueCdUiSpec(this, parameters);
        }
    }
    
    public class CueCdUiSpec:GameplayCueDurationalSpec<CueCdUi>
    {
        private readonly CueCdUi _cueCdUi;
        public CueCdUiSpec(CueCdUi cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            _cueCdUi = cue;
        }

        public override void OnAdd()
        {
            
        }

        public override void OnRemove()
        {
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
        }
    }
}