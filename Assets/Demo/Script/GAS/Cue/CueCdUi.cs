using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
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
            switch (cue.cdType)
            {
                case CdType.DodgeCd:
                    XUI.M.VM<MainUIVM>().DodgeCDVisible.Value = true;
                    break;
                case CdType.FirBulletCd:
                    XUI.M.VM<MainUIVM>().FireBulletCDVisible.Value = true;
                    break;
                case CdType.PlayerBuff:
                    XUI.M.VM<MainUIVM>().PlayerBuffVisible.Value = true;
                    break;
                case CdType.BossBuff:
                    XUI.M.VM<MainUIVM>().BossBuffVisible.Value = true;
                    break;
            }
        }

        public override void OnRemove()
        {
            switch (cue.cdType)
            {
                case CdType.DodgeCd:
                    XUI.M.VM<MainUIVM>().DodgeCDVisible.Value = false;
                    break;
                case CdType.FirBulletCd:
                    XUI.M.VM<MainUIVM>().FireBulletCDVisible.Value = false;
                    break;
                case CdType.PlayerBuff:
                    XUI.M.VM<MainUIVM>().PlayerBuffVisible.Value = false;
                    break;
                case CdType.BossBuff:
                    XUI.M.VM<MainUIVM>().BossBuffVisible.Value = false;
                    break;
            }
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
            float value = _parameters.sourceGameplayEffectSpec.DurationRemaining();
            float max = _parameters.sourceGameplayEffectSpec.Duration;
            switch (cue.cdType)
            {
                case CdType.DodgeCd:
                    XUI.M.VM<MainUIVM>().UpdateDodgeCD(value, max);
                    break;
                case CdType.FirBulletCd:
                    XUI.M.VM<MainUIVM>().UpdateFireBulletCD(value, max);
                    break;
                case CdType.PlayerBuff:
                    XUI.M.VM<MainUIVM>().UpdatePlayerBuffTime(value, max);
                    break;
                case CdType.BossBuff:
                    XUI.M.VM<MainUIVM>().UpdateBossBuffTime(value, max);
                    break;
            }
        }
    }
}