namespace GAS.Runtime
{
    /// <summary>  
    /// 属性值解析器接口。  
    /// 面向 OOP 层的 AbilitySystemCell，用户无需感知 ECS。  
    /// </summary>  
    public interface IAttributeValueResolver
    {
        float Resolve(AbilitySystemCell asc, int attrSetCode, int attrCode);
    }
}