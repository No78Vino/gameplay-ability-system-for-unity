using Demo.Script.Element;
using GAS.General;
using Loxodon.Framework.Extension;

namespace Demo.Script.UI
{
    public class MainUIVM:ViewModelCommon
    {
        public ObservableVariable<float> playerHp = new ObservableVariable<float>();
        public ObservableVariable<float> playerHpMax = new ObservableVariable<float>();
        
        public ObservableVariable<float> playerMp = new ObservableVariable<float>();
        public ObservableVariable<float> playerMpMax = new ObservableVariable<float>();
        
        public ObservableVariable<float> playerStamina = new ObservableVariable<float>();
        public ObservableVariable<float> playerStaminaMax = new ObservableVariable<float>();
        
        public ObservableVariable<float> playerPosture = new ObservableVariable<float>();
        public ObservableVariable<float> playerPostureMax = new ObservableVariable<float>();
        
        public  ObservableVariable<bool> BossUiVisible = new ObservableVariable<bool>();
        public ObservableVariable<string> BossName = new ObservableVariable<string>();
        
        public ObservableVariable<float> BossHp = new ObservableVariable<float>();
        public ObservableVariable<float> BossHpMax = new ObservableVariable<float>();
        
        public ObservableVariable<float> BossPosture = new ObservableVariable<float>();
        public ObservableVariable<float> BossPostureMax = new ObservableVariable<float>();
        
        public ObservableVariable<string> FireBulletName = new ObservableVariable<string>();
        public ObservableVariable<float> FireBulletCDMax = new ObservableVariable<float>();
        public ObservableVariable<float> FireBulletCD = new ObservableVariable<float>();
        public ObservableVariable<bool> FireBulletCDVisible = new ObservableVariable<bool>();
        
        public ObservableVariable<string> DodgeName = new ObservableVariable<string>();
        public ObservableVariable<float> DodgeCDMax = new ObservableVariable<float>();
        public ObservableVariable<float> DodgeCD = new ObservableVariable<float>();
        public ObservableVariable<bool> DodgeCDVisible = new ObservableVariable<bool>();
        
        public ObservableVariable<string> PlayerBuffName = new ObservableVariable<string>();
        public ObservableVariable<float> PlayerBuffTimeMax = new ObservableVariable<float>();
        public ObservableVariable<float> PlayerBuffTime = new ObservableVariable<float>();
        public ObservableVariable<bool> PlayerBuffVisible = new ObservableVariable<bool>();
        
        public ObservableVariable<string> BossBuffName = new ObservableVariable<string>();
        public ObservableVariable<float> BossBuffTimeMax = new ObservableVariable<float>();
        public ObservableVariable<float> BossBuffTime = new ObservableVariable<float>();
        public ObservableVariable<bool> BossBuffVisible = new ObservableVariable<bool>();
        
        public MainUIVM()
        {
            playerHpMax.Value = Player.HpMax;
            playerHp.Value = Player.HpMax;
            playerMpMax.Value = Player.MpMax;
            playerMp.Value = Player.MpMax;
            playerStaminaMax.Value = 100;
            playerStamina.Value = 100;
            playerPostureMax.Value = Player.PostureMax;
            playerPosture.Value = Player.PostureMax;
            
            BossName.Value = "【Boss】矩形和圆形";
            BossHpMax.Value = BossBladeFang.HpMax;
            BossHp.Value = BossBladeFang.HpMax;
            BossPostureMax.Value = BossBladeFang.PostureMax;
            BossPosture.Value = BossBladeFang.PostureMax;
            
            FireBulletCDVisible.Value = false;
            DodgeCDVisible.Value = false;
            PlayerBuffVisible.Value = false;
            BossBuffVisible.Value = false;
            FireBulletName.Value = "火球术";
            DodgeName.Value = "闪避";
            PlayerBuffName.Value = "失衡";
            BossBuffName.Value = "失衡";
        }
        
        public void UpdateStamina(float value)
        {
            playerStamina.Value = value;
        }
        
        public void UpdatePosture(float value)
        {
            playerPosture.Value = value;
        }
        
        public void UpdateHp(float value)
        {
            playerHp.Value = value;
        }
        
        public void UpdateBossPosture(float value)
        {
            BossPosture.Value = value;
        }
        
        public void UpdateBossHp(float value)
        {
            BossHp.Value = value;
        }
        
        public void UIShake(bool bigOrSmall)
        {
            if (bigOrSmall)
                transitionRequest.Raise("shakeBig");
            else
                transitionRequest.Raise("shakeSmall");
        }
        
        public void ShowBossUI()
        {
            BossUiVisible.Value = true;
        }
        
        public void HideBossUI()
        {
            BossUiVisible.Value = false;
        }

        public void ResetUI()
        {
            // 刷新player血条
            UpdateStamina(Player.StaminaMax);
            UpdatePosture(0);
            UpdateHp(Player.HpMax);

            // 刷新Boss血条
            UpdateBossPosture(0);
            UpdateBossHp(BossBladeFang.HpMax);
            
            HideBossUI();
        }
        
        public void UpdateFireBulletCD(float value,float max)
        {
            FireBulletCD.Value = value;
            FireBulletCDMax.Value = max;
        }
        
        public void UpdateDodgeCD(float value,float max)
        {
            DodgeCD.Value = value;
            DodgeCDMax.Value = max;
        }
        
        public void UpdatePlayerBuffTime(float value,float max)
        {
            PlayerBuffTime.Value = value;
            PlayerBuffTimeMax.Value = max;
        }
        
        public void UpdateBossBuffTime(float value,float max)
        {
            BossBuffTime.Value = value;
            BossBuffTimeMax.Value = max;
        }
    }
}