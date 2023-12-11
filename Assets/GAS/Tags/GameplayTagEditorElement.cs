using GAS.Runtime.Tags;
using UnityEditor.TreeViewExamples;

namespace GAS.Editor.Tags
{
    public class GameplayTagEditorElement:TreeElement
    {
        public string fullName;
        public GameplayTagEditorElement (GameplayTag gameplayTag, int depth, int id) : base (gameplayTag.ShortName, depth, id)
        {
            fullName = gameplayTag.name;
        }
    }
}