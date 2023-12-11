using GAS.Runtime.Tags;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;

namespace GAS.Editor.Tags
{
    class GameplayTagsTreeView : TreeViewWithTreeModel<GameplayTagEditorElement>
    {
        public GameplayTagsTreeView(TreeViewState state, TreeModel<GameplayTagEditorElement> model)
            : base(state, model)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
        }
    }
}