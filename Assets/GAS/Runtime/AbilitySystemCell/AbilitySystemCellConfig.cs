namespace GAS.Runtime
{
    public struct AbilitySystemCellConfig
    {
        public AbilitySystemCellConfig(int[] baseTags, AttributeSetConfig[] attrSets, AbilityConfig[] baseAbilities, int level = 1)
        {
            BaseTags = baseTags;
            AttrSets = attrSets;
            BaseAbilities = baseAbilities;
            Level = level;
        }

        public int[] BaseTags { get; private set; }

        public AttributeSetConfig[] AttrSets { get; private set; }

        public AbilityConfig[] BaseAbilities { get; private set; }

        public int Level { get; private set; }

        public void SetBaseTags(int[] baseTags)
        {
            BaseTags = baseTags;
        }

        public void SetAttrSets(AttributeSetConfig[] attrSets)
        {
            AttrSets = attrSets;
        }

        public void SetBaseAbilities(AbilityConfig[] baseAbilities)
        {
            BaseAbilities = baseAbilities;
        }

        public void SetLevel(int level)
        {
            Level = level;
        }
    }
}