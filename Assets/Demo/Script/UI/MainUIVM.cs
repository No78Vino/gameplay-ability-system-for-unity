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
            playerHpMax.Value = 100;
            playerHp.Value = 100;
            playerMpMax.Value = 100;
            playerMp.Value = 100;
            playerStaminaMax.Value = 100;
            playerStamina.Value = 100;
            playerPostureMax.Value = 100;
            playerPosture.Value = 100;
            
            BossName.Value = "【Boss】爪刃";
            BossHpMax.Value = 100;
            BossHp.Value = 100;
            BossPostureMax.Value = 100;
            BossPosture.Value = 100;
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
    }
}