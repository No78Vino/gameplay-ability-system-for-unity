namespace GAS.Runtime  
{
    /// <summary>  
    /// 默认属性值解析器。  
    /// 直接使用 AbilitySystemCell 的 OOP 接口读取属性，用户无需感知 ECS。  
    /// </summary>  
    public sealed class DefaultAttributeValueResolver : IAttributeValueResolver
    {
        public float Resolve(AbilitySystemCell asc, int attrSetCode, int attrCode)
            => asc.GetAttrCurrentValue(attrSetCode, attrCode);
    }
}