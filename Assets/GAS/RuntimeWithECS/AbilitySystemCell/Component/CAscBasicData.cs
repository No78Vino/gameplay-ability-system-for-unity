using Unity.Entities;

namespace GAS.Runtime
{
    /// <summary>
    /// ASC的基础数据Component，包含了所有ASC通用数值字段
    /// </summary>
    public struct CAscBasicData : IComponentData
    {
        public int Level;  // ASC当前等级
    }
}