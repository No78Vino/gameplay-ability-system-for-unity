using System.Collections.Generic;
using GAS.Runtime.Tags;
using GAS.Tags;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeDataModel;
using UnityEngine;

namespace GAS.Editor.Tags
{
    [CustomEditor(typeof(GameplayTagsAsset))]
    public class GameplayTagsTreeAssetEditor : UnityEditor.Editor
    {
        private const string kSessionStateKeyPrefix = "TVS";
        private SearchField m_SearchField;
        private GameplayTagTreeView m_TreeView;

        private GameplayTagsAsset asset => (GameplayTagsAsset)target;

        private void OnEnable()
        {
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            var treeViewState = new TreeViewState();
            var jsonState = SessionState.GetString(kSessionStateKeyPrefix + asset.GetInstanceID(), "");
            if (!string.IsNullOrEmpty(jsonState))
                JsonUtility.FromJsonOverwrite(jsonState, treeViewState);
            var treeModel = new TreeModel<GameplayTagTreeElement>(asset.GameplayTagTreeElements);
            m_TreeView = new GameplayTagTreeView(treeViewState, treeModel, asset);
            m_TreeView.beforeDroppingDraggedItems += OnBeforeDroppingDraggedItems;
            m_TreeView.Reload();

            m_SearchField = new SearchField();
            m_SearchField.downOrUpArrowKeyPressed += m_TreeView.SetFocusAndEnsureSelectedItem;

            if (!m_TreeView.treeModel.Root.HasChildren) CreateFirstTag();
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;

            SessionState.SetString(kSessionStateKeyPrefix + asset.GetInstanceID(),
                JsonUtility.ToJson(m_TreeView.state));
        }

        private void OnUndoRedoPerformed()
        {
            if (m_TreeView != null)
            {
                m_TreeView.treeModel.SetData(asset.GameplayTagTreeElements);
                m_TreeView.Reload();
            }
        }

        private void OnBeforeDroppingDraggedItems(IList<TreeViewItem> draggedRows)
        {
            Undo.RecordObject(asset,
                string.Format("Moving {0} Tag{1}", draggedRows.Count, draggedRows.Count > 1 ? "s" : ""));
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(5f);
            ToolBar();
            GUILayout.Space(3f);

            const float topToolbarHeight = 20f;
            const float spacing = 2f;
            var totalHeight = m_TreeView.totalHeight + topToolbarHeight + 2 * spacing;
            var rect = GUILayoutUtility.GetRect(0, 10000, 0, totalHeight);
            var toolbarRect = new Rect(rect.x, rect.y, rect.width, topToolbarHeight);
            var multiColumnTreeViewRect = new Rect(rect.x, rect.y + topToolbarHeight + spacing, rect.width,
                rect.height - topToolbarHeight - 2 * spacing);
            SearchBar(toolbarRect);
            DoTreeView(multiColumnTreeViewRect);
        }

        private void SearchBar(Rect rect)
        {
            m_TreeView.searchString = m_SearchField.OnGUI(rect, m_TreeView.searchString);
        }

        private void DoTreeView(Rect rect)
        {
            m_TreeView.OnGUI(rect);
        }

        private void ToolBar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var style = "miniButton";
                if (GUILayout.Button("Expand All", style)) m_TreeView.ExpandAll();

                if (GUILayout.Button("Collapse All", style)) m_TreeView.CollapseAll();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Create Tag", style)) CreateTag();

                if (GUILayout.Button("Remove Tags", style)) RemoveTags();
                
                if (GUILayout.Button("Generate TagSum Code", style)) GameplayTagSumCollectionGenerator.Gen();
            }
        }

        private void AddTag(string tagName)
        {
            Undo.RecordObject(asset, "Add Item To Asset");
            var selection = m_TreeView.GetSelection();
            TreeElement parent = (selection.Count == 1 ? m_TreeView.treeModel.Find(selection[0]) : null) ??
                                 m_TreeView.treeModel.Root;
            var depth = parent != null ? parent.Depth + 1 : 0;
            var id = m_TreeView.treeModel.GenerateUniqueID();
            var element = new GameplayTagTreeElement(tagName, depth, id);
            m_TreeView.treeModel.AddElement(element, parent, 0);

            // Select newly created element
            m_TreeView.SetSelection(new[] { id }, TreeViewSelectionOptions.RevealAndFrame);
            SaveAsset();
        }

        public void CreateTag()
        {
            Undo.RecordObject(asset, "Add Item To Asset");
            CreateTagWindow.OpenWindow(AddTag);
        }

        public void RemoveTags()
        {
            var result = EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to REMOVE these tags?",
                "Yes", "No");

            if (result)
            {
                Undo.RecordObject(asset, "Remove Tag From Asset");
                var selection = m_TreeView.GetSelection();
                m_TreeView.treeModel.RemoveElements(selection);
                SaveAsset();
            }
        }

        private void CreateFirstTag()
        {
            AddTag("Ability");
        }

        private void SaveAsset()
        {
            asset.CacheTags();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
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