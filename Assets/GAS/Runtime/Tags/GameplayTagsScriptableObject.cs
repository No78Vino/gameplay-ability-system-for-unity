using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

namespace GAS.Runtime.Tags
{
    [CreateAssetMenu( menuName = "GAS/Tag", order = 0)]
    public class GameplayTagsScriptableObject : ScriptableObject
    {
        [SerializeField] private GameplayTagsScriptableObject Parent;
        [SerializeField] private int ancestorsToFind = 4;
        public GameplayTag TagData;
        
        public bool IsDescendantOf(GameplayTagsScriptableObject other, int nSearchLimit = 4)
        {
            int i = 0;
            GameplayTagsScriptableObject tags = Parent;
            while (nSearchLimit > i++)
            {
                // tag will be invalid once we are at the root ancestor
                if (!tags) return false;

                // Match found, so we can return true
                if (tags == other) return true;

                // No match found, so try again with the next ancestor
                tags = tags.Parent;
            }


            // If we've exhausted the search limit, no ancestor was found
            return false;
        }

        public void OnValidate()
        {
            UpdateCache();
        }

        private void UpdateCache()
        {
            this.TagData = Build(ancestorsToFind);
        }

        public GameplayTag Build(int nSearchLimit = 4)
        {
            if (nSearchLimit < 0) nSearchLimit = ancestorsToFind;

            var ancestors = new List<int>();
            var parent = this.Parent;
            for (var i = 0; i < nSearchLimit; i++)
            {
                ancestors.Add(parent?.GetInstanceID() ?? 0);
                // Leave the loop early if there no further ancestors
                parent = parent?.Parent;
                i = math.select(i, nSearchLimit, parent == null);
            }

            return new GameplayTag()
            {
                tag = this.GetInstanceID(),
                ancestors = ancestors.ToArray()
            };
        }

        [Serializable]
        public struct GameplayTag
        {
            private string _name;
            public string Name => _name;
            
            
            public int tag;
            public int[] ancestors;

            public bool IsDescendantOf(GameplayTag other)
            {
                return other.ancestors.Contains(tag);
            }

            public override bool Equals(object obj)
            {
                return obj is GameplayTag && this == (GameplayTag)obj;
            }

            public override int GetHashCode()
            {
                return tag.GetHashCode();
            }
            public static bool operator ==(GameplayTag x, GameplayTag y)
            {
                return x.tag == y.tag;
            }

            public static bool operator !=(GameplayTag x, GameplayTag y)
            {
                return x.tag != y.tag;
            }
        }
    }
}