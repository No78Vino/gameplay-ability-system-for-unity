using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    /// <summary>
    /// 新版GE Spec其实是对GE Entity的管理单位
    /// 特意装了这一层是为了方便开发者用面向对象的思维处理GE的实例
    /// </summary>
    public class NewGameplayEffectSpec
    {
        private Entity _entity;
        
    }
}