using GAS.Editor.Validation;

#if UNITY_EDITOR
namespace GAS.Editor
{
    using System.Collections.Generic;
    using GAS.Editor.General;
    using Editor;
    using GAS.General;
    using UnityEditor;
    using UnityEditor.IMGUI.Controls;
    using UnityEditor.TreeDataModel;
    using UnityEngine;

    [CustomEditor(typeof(GameplayTagsAsset))]
    public class GameplayTagsTreeAssetEditor : UnityEditor.Editor
    {
        private const string KSessionStateKeyPrefix = "TVS";
        private SearchField _searchField;
        private GameplayTagTreeView _treeView;

        private GameplayTagsAsset Asset => (GameplayTagsAsset)target;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            var treeViewState = new TreeViewState();
            var jsonState = SessionState.GetString(KSessionStateKeyPrefix + Asset.GetInstanceID(), "");
            if (!string.IsNullOrEmpty(jsonState))
                JsonUtility.FromJsonOverwrite(jsonState, treeViewState);
            var treeModel = new TreeModel<GameplayTagTreeElement>(Asset.GameplayTagTreeElements);
            _treeView = new GameplayTagTreeView(treeViewState, treeModel, Asset);
            _treeView.beforeDroppingDraggedItems += OnBeforeDroppingDraggedItems;
            _treeView.Reload();

            _searchField = new SearchField();
            _searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;

            if (!_treeView.treeModel.Root.HasChildren) CreateFirstTag();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            SessionState.SetString(KSessionStateKeyPrefix + Asset.GetInstanceID(),
                JsonUtility.ToJson(_treeView.state));
        }

        private void OnUndoRedoPerformed()
        {
            if (_treeView != null)
            {
                _treeView.treeModel.SetData(Asset.GameplayTagTreeElements);
                _treeView.Reload();
            }
        }

        private void OnBeforeDroppingDraggedItems(IList<TreeViewItem> draggedRows)
        {
            Undo.RecordObject(Asset,
                $"Moving {draggedRows.Count} Tag{(draggedRows.Count > 1 ? "s" : "")}");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            ToolBar();
            GUILayout.Space(3f);

            const float topToolbarHeight = 20f;
            const float spacing = 2f;
            var totalHeight = _treeView.totalHeight + topToolbarHeight + 2 * spacing;
            var rect = GUILayoutUtility.GetRect(0, 10000, 0, totalHeight);
            var toolbarRect = new Rect(rect.x, rect.y, rect.width, topToolbarHeight);
            var multiColumnTreeViewRect = new Rect(rect.x, rect.y + topToolbarHeight + spacing, rect.width,
                rect.height - topToolbarHeight - 2 * spacing);
            SearchBar(toolbarRect);
            DoTreeView(multiColumnTreeViewRect);
        }

        private void SearchBar(Rect rect)
        {
            _treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString);
        }

        private void DoTreeView(Rect rect)
        {
            _treeView.OnGUI(rect);
        }

        private void ToolBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                if (GUILayout.Button(GASTextDefine.BUTTON_ExpandAllTag, style)) _treeView.ExpandAll();

                if (GUILayout.Button(GASTextDefine.BUTTON_CollapseAllTag, style)) _treeView.CollapseAll();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button(GASTextDefine.BUTTON_AddTag, style)) CreateTag();

                if (GUILayout.Button(GASTextDefine.BUTTON_RemoveTag, style)) RemoveTags();

                if (GUILayout.Button(GASTextDefine.BUTTON_GenTagCode, style)) GenCode();
            }
        }

        private void AddTag(string tagName)
        {
            Undo.RecordObject(Asset, "Add Item To Asset");
            var selection = _treeView.GetSelection();
            TreeElement parent = (selection.Count == 1 ? _treeView.treeModel.Find(selection[0]) : null) ??
                                 _treeView.treeModel.Root;
            var depth = parent != null ? parent.Depth + 1 : 0;
            var id = _treeView.treeModel.GenerateUniqueID();
            var element = new GameplayTagTreeElement(tagName, depth, id);
            _treeView.treeModel.AddElement(element, parent, 0);

            // Select newly created element
            _treeView.SetSelection(new[] { id }, TreeViewSelectionOptions.RevealAndFrame);
            SaveAsset();
        }

        public void CreateTag()
        {
            Undo.RecordObject(Asset, "Add Item To Asset");
            StringEditWindow.OpenWindow("Tag", "", Validations.ValidateVariableName, AddTag, "Create new Tag");
        }

        public void RemoveTags()
        {
            var result = EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to REMOVE these tags?",
                "Yes", "No");

            if (result)
            {
                Undo.RecordObject(Asset, "Remove Tag From Asset");
                var selection = _treeView.GetSelection();
                _treeView.treeModel.RemoveElements(selection);
                SaveAsset();
            }
        }

        private void CreateFirstTag()
        {
            AddTag("Ability");
        }

        private void SaveAsset()
        {
            Asset.CacheTags();
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssets();
        }

        void GenCode()
        {
            GTagLibGenerator.Gen();
            AssetDatabase.Refresh();
        }

        private class GameplayTagTreeView : TreeViewWithTreeModel<GameplayTagTreeElement>
        {
            private readonly GameplayTagsAsset _asset;

            public GameplayTagTreeView(TreeViewState state, TreeModel<GameplayTagTreeElement> model,
                GameplayTagsAsset asset)
                : base(state, model)
            {
                showBorder = true;
                showAlternatingRowBackgrounds = true;
                _asset = asset;
            }

            public override void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows,
                GameplayTagTreeElement parent, int insertIndex)
            {
                base.OnDropDraggedElementsAtIndex(draggedRows, parent, insertIndex);
                _asset.CacheTags();
                EditorUtility.SetDirty(_asset);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
#endif