namespace GAS.Runtime
{
    public struct AbilitySystemCellConfig
    {
        public AbilitySystemCellConfig(int[] baseTags, AttrSetConfig[] attrSets, AbilityConfig[] baseAbilities, int level = 1)
        {
            BaseTags = baseTags;
            AttrSets = attrSets;
            BaseAbilities = baseAbilities;
            Level = level;
        }

        public int[] BaseTags { get; private set; }

        public AttrSetConfig[] AttrSets { get; private set; }

        public AbilityConfig[] BaseAbilities { get; private set; }

        public int Level { get; private set; }

        public void SetBaseTags(int[] baseTags)
        {
            BaseTags = baseTags;
        }

        public void SetAttrSets(AttrSetConfig[] attrSets)
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