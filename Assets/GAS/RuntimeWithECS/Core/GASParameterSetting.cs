namespace GAS.Runtime
{
    public static class GASParameterSetting
    {
        /// <summary>
        ///  A maximum of 100 abilities that an ASC unit can hold
        ///  单个ASC单位可持有的最大能力数量为100
        /// 这是合理的上限，因为能力数量过多会导致性能问题，同时也不建议一个ASC单位持有过多的能力
        /// 如果需要更多的能力，可以二次开发时修改这个值（但是不推荐）
        /// </summary>
        public const int ASC_MAX_ABILITY_COUNT = 100;
        
        
        /// <summary>
        ///  A maximum of 500 gameplay effects that an ASC unit can hold
        ///  单个ASC单位可持有的最大GE数量为500
        ///  这是合理的上限，因为GE数量过多会导致性能问题，同时也不建议一个ASC单位持有过多的GE
        ///  如果需要更多的GE，可以二次开发时修改这个值（但是不推荐）
        /// </summary>
        public const int ASC_MAX_GAMEPLAY_EFFECT_COUNT = 500;
    }
}