using GAS.General;
using Loxodon.Framework.Extension;

namespace Demo.Script.UI
{
    public class MainUIVM:ViewModelCommon
    {
        public ObservableVariable<float> playerHp = new ObservableVariable<float>();
        public ObservableVariable<float> playerHpMax = new ObservableVariable<float>();
        
        public ObservableVariable<bool> healingBuff = new ObservableVariable<bool>();
        
        public ObservableVariable<float> fireballCd = new ObservableVariable<float>();
        public ObservableVariable<float> fireballCdMax = new ObservableVariable<float>();
        public ObservableVariable<string> fireballSkillName = new ObservableVariable<string>();
        public ObservableVariable<string> fireballCdCount = new ObservableVariable<string>();
        
        public ObservableVariable<float> freezebeamCd = new ObservableVariable<float>();
        public ObservableVariable<float> freezebeamCdMax = new ObservableVariable<float>();
        public ObservableVariable<string> freezebeamSkillName = new ObservableVariable<string>();
        public ObservableVariable<string> freezebeamCdCount = new ObservableVariable<string>();

        public MainUIVM()
        {
            
        }
    }
}