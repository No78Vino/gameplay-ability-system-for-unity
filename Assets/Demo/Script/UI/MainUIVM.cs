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
        
        public ObservableVariable<string> BossName = new ObservableVariable<string>();
        
        public ObservableVariable<float> BossHp = new ObservableVariable<float>();
        public ObservableVariable<float> BossHpMax = new ObservableVariable<float>();
        
        public ObservableVariable<float> BossPosture = new ObservableVariable<float>();
        public ObservableVariable<float> BossPostureMax = new ObservableVariable<float>();

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
    }
}