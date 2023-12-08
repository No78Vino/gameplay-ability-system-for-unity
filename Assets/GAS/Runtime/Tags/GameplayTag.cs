using System;
using System.Collections.Generic;
using System.Linq;

namespace GAS.Runtime.Tags
{
    [Serializable]
    public struct GameplayTag
    {
        private int[] _ancestors;
        private List<int> _descendants;

        public GameplayTag(string name)
        {
            Name = name;
            HashCode = name.GetHashCode();


            var tags = name.Split('.');
            if (tags.Length > GasDefine.GAS_TAG_MAX_GENERATIONS)
            {
                throw new Exception($"GameplayTag {name} has more than {GasDefine.GAS_TAG_MAX_GENERATIONS} generations");
            }

            _ancestors = new int[tags.Length-1];
            int i = 0;
            string ancestorTag = "";
            while (i < tags.Length - 1)
            {
                ancestorTag += tags[i];
                _ancestors[i] = ancestorTag.GetHashCode();
                ancestorTag += ".";
                i++;
            }

            _descendants = new List<int>();
        }

        /// <summary>
        /// Only For Show. 
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Actually ,Use the hash code for compare.
        /// </summary>
        public int HashCode { get; private set; }

        public void AddDescendant(GameplayTag descendant)
        {
            _descendants.Add(descendant.HashCode);
        }
        
        public bool IsDescendantOf(GameplayTag other)
        {
            return other._ancestors.Contains(HashCode);
        }

        public override bool Equals(object obj)
        {
            return obj is GameplayTag tag && this == tag;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public static bool operator ==(GameplayTag x, GameplayTag y)
        {
            return x.HashCode == y.HashCode;
        }

        public static bool operator !=(GameplayTag x, GameplayTag y)
        {
            return x.HashCode != y.HashCode;
        }
    }
}