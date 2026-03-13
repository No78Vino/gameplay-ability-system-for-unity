using Unity.Entities;  
  
namespace GAS.Runtime  
{  
    /// <summary>  
    /// 运行时追踪GE授予的Ability Entity引用  
    /// 挂载在GE Entity上  
    /// </summary>  
    public class MCGrantedAbilityRuntime : IComponentData  
    {  
        public Entity[] GrantedAbilityEntities;  
          
        public MCGrantedAbilityRuntime()  
        {  
        }  
          
        public MCGrantedAbilityRuntime(Entity[] grantedAbilityEntities)  
        {  
            GrantedAbilityEntities = grantedAbilityEntities;  
        }  
    }  
}