using System;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// <remarks>
    /// 实现IEquatable接口, 避免当作为Dictionary的Key时产生GC Alloc.
    /// </remarks>>
    /// </summary>
    [Serializable]
    public struct GameplayTag : IEquatable<GameplayTag>
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private int _hashCode;

        [SerializeField]
        private string _shortName;

        [SerializeField]
        private int[] _ancestorHashCodes;

        [SerializeField]
        private string[] _ancestorNames;

        public GameplayTag(string name)
        {
            _name = name;
            _hashCode = name.GetHashCode();

            var tags = name.Split('.');
            // if (tags.Length > GasDefine.GAS_TAG_MAX_GENERATIONS)
            //     throw new Exception(
            //         $"GameplayTag {name} has more than {GasDefine.GAS_TAG_MAX_GENERATIONS} generations");

            _ancestorNames = new string[tags.Length - 1];
            _ancestorHashCodes = new int[tags.Length - 1];
            var i = 0;
            var ancestorTag = "";
            while (i < tags.Length - 1)
            {
                ancestorTag += tags[i];
                _ancestorHashCodes[i] = ancestorTag.GetHashCode();
                _ancestorNames[i] = ancestorTag;
                ancestorTag += ".";
                i++;
            }

            _shortName = tags[^1];
        }

        /// <summary>
        /// Only For Show.
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// Only For Show.
        /// </summary>
        public string ShortName => _shortName;

        /// <summary>
        /// Actually ,Use the hash code for compare.
        /// </summary>
        public int HashCode => _hashCode;

        public string[] AncestorNames => _ancestorNames;

        public bool Root => _ancestorHashCodes.Length == 0;

        public int[] AncestorHashCodes => _ancestorHashCodes;

        public bool IsDescendantOf(in GameplayTag other) => other.HasTag(this);

        public bool Equals(GameplayTag other) => this == other;

        public override bool Equals(object obj) => obj is GameplayTag tag && this == tag;

        public override int GetHashCode() => HashCode;

        public static bool operator ==(GameplayTag x, GameplayTag y) => x.HashCode == y.HashCode;

        public static bool operator !=(GameplayTag x, GameplayTag y) => x.HashCode != y.HashCode;

        public readonly bool HasTag(in GameplayTag tag)
        {
            foreach (var ancestorHashCode in _ancestorHashCodes)
                if (ancestorHashCode == tag.HashCode)
                    return true;

            return this == tag;
        }
    }
}