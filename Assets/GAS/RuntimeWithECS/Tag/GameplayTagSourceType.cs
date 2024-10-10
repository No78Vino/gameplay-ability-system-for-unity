namespace GAS.RuntimeWithECS.Tag
{
    public enum GameplayTagSourceType
    {
        None, // 设计上临时Tag来源不可能是None。如果是None，那这个临时Tag其实等价于固定Tag
        GameplayEffect,
        Ability,
    }
}