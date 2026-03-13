using System;  
  
namespace GAS.Runtime  
{  
    [Serializable]  
    public class TaskDoCooldown : AbilityTaskBase<XParamNone>  
    {  
        protected override void OnBegin(int startFrame)  
        {  
            AbilityUtil.DoCooldown(_logic.GetAbilityEntity());  
        }  
  
        public TaskDoCooldown(AbilityLogicBase logic) : base(logic)  
        {  
        }  
    }  
}