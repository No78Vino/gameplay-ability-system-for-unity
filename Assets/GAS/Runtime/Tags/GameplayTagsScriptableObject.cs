// using System.Collections.Generic;
// using Unity.Mathematics;
// using UnityEngine;
//
// namespace GAS.Runtime.Tags
// {
//     public class GameplayTagsScriptableObject : ScriptableObject
//     {
//         [SerializeField] private GameplayTagsScriptableObject Parent;
//         [SerializeField] private int ancestorsToFind = 4;
//         public GameplayTag TagData;
//
//         public void OnValidate()
//         {
//             UpdateCache();
//         }
//
//         public bool IsDescendantOf(GameplayTagsScriptableObject other, int nSearchLimit = 4)
//         {
//             var i = 0;
//             var tags = Parent;
//             while (nSearchLimit > i++)
//             {
//                 // tag will be invalid once we are at the root ancestor
//                 if (!tags) return false;
//
//                 // Match found, so we can return true
//                 if (tags == other) return true;
//
//                 // No match found, so try again with the next ancestor
//                 tags = tags.Parent;
//             }
//
//
//             // If we've exhausted the search limit, no ancestor was found
//             return false;
//         }
//
//         private void UpdateCache()
//         {
//             TagData = new GameplayTag(name);
//         }
//
//         public GameplayTag Build(int nSearchLimit = 4)
//         {
//             return 
//             if (nSearchLimit < 0) nSearchLimit = ancestorsToFind;
//
//             var ancestors = new List<int>();
//             var parent = Parent;
//             for (var i = 0; i < nSearchLimit; i++)
//             {
//                 ancestors.Add(parent?.GetInstanceID() ?? 0);
//                 // Leave the loop early if there no further ancestors
//                 parent = parent?.Parent;
//                 i = math.select(i, nSearchLimit, parent == null);
//             }
//
//             return new GameplayTag
//             {
//                 HashCode = GetInstanceID(),
//                 ancestors = ancestors.ToArray()
//             };
//         }
//     }
// }