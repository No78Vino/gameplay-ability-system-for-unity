using Unity.Entities;

namespace GAS.RuntimeWithECS.GameplayEffect
{
    /// <summary>
    /// 新版GE Spec其实是对GE Entity的管理单位
    /// 特意装了这一层是为了方便开发者用面向对象的思维处理GE的实例
    /// GE Spec提供所有类型组件的数据修改接口。
    /// 【注意！】
    /// 持有组件的类型，建议在初始化就固定好。不建议spec生成完再动态的增删组件。
    /// 随意的组件增删对于System的运作逻辑都是存在的隐患的。
    /// 数据修改都是没问题的。
    /// </summary>
    public class NewGameplayEffectSpec
    {
        public Entity Entity { get; set; }

        public NewGameplayEffectSpec(GameplayEffectComponentConfig[] componentConfigs)
        {
            Entity = GEUtil.CreateGameplayEffectEntity(componentConfigs);
        }
        
        public NewGameplayEffectSpec(Entity geEntity)
        {
            Entity = geEntity;
        }
    }
}