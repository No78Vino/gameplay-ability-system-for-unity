using Unity.Entities;

namespace GAS.RuntimeWithECS.AbilitySystemCell.Component
{
    /// <summary>
    /// ASC的基础数据Component，包含了所有ASC通用数值字段
    /// </summary>
    public struct BasicDataComponent : IComponentData
    {
        public int Level;  // ASC当前等级
    }
}