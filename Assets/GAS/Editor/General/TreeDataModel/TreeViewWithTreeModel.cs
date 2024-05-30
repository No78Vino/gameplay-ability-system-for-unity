#if UNITY_EDITOR
namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor.IMGUI.Controls;
    using UnityEngine;
    using Object = UnityEngine.Object;

    
    internal class TreeViewItem<T> : TreeViewItem where T : ExTreeElement
    {
        public TreeViewItem(int id, int depth, string displayName, T data) : base(id, depth, displayName)
        {
            this.data = data;
        }

        public T data { get; set; }
    }

    internal class TreeViewWithTreeModel<T> : TreeView where T : ExTreeElement
    {
        // Dragging
        //-----------

        private const string k_GenericDragID = "GenericDragColumnDragging";
        private readonly List<TreeViewItem> m_Rows = new List<TreeViewItem>(100);


        public TreeViewWithTreeModel(TreeViewState state, ExTreeModel<T> model) : base(state)
        {
            Init(model);
        }

        public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, ExTreeModel<T> model)
            : base(state, multiColumnHeader)
        {
            Init(model);
        }

        public ExTreeModel<T> ExTreeModel { get; private set; }

        public event Action treeChanged;
        public event Action<IList<TreeViewItem>> beforeDroppingDraggedItems;

        private void Init(ExTreeModel<T> model)
        {
            ExTreeModel = model;
            ExTreeModel.modelChanged += ModelChanged;
        }

        private void ModelChanged()
        {
            if (treeChanged != null)
                treeChanged();

            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var depthForHiddenRoot = -1;
            return new TreeViewItem<T>(ExTreeModel.Root.ID, depthForHiddenRoot, ExTreeModel.Root.Name, ExTreeModel.Root);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (ExTreeModel.Root == null) Debug.LogError("tree model root is null. did you call SetData()?");

            m_Rows.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                Search(ExTreeModel.Root, searchString, m_Rows);
            }
            else
            {
                if (ExTreeModel.Root.HasChildren)
                    AddChildrenRecursive(ExTreeModel.Root, 0, m_Rows);
            }

            // We still need to setup the child parent information for the rows since this 
            // information is used by the TreeView internal logic (navigation, dragging etc)
            SetupParentsAndChildrenFromDepths(root, m_Rows);

            return m_Rows;
        }

        private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (T child in parent.Children)
            {
                var item = new TreeViewItem<T>(child.ID, depth, child.Name, child);
                newRows.Add(item);

                if (child.HasChildren)
                {
                    if (IsExpanded(child.ID))
                        AddChildrenRecursive(child, depth + 1, newRows);
                    else
                        item.children = CreateChildListForCollapsedParent();
                }
            }
        }

        private void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Invalid search: cannot be null or empty", "search");

            const int kItemDepth = 0; // tree is flattened when searching

            var stack = new Stack<T>();
            foreach (var element in searchFromThis.Children)
                stack.Push((T)element);
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                // Matches search?
                if (current.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    result.Add(new TreeViewItem<T>(current.ID, kItemDepth, current.Name, current));

                if (current.Children != null && current.Children.Count > 0)
                    foreach (var element in current.Children)
                        stack.Push((T)element);
            }

            SortSearchResult(result);
        }

        protected virtual void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) =>
                EditorUtility.NaturalCompare(x.displayName,
                    y.displayName)); // sort by displayName by default, can be overriden for multicolumn solutions
        }

        protected override IList<int> GetAncestors(int id)
        {
            return ExTreeModel.GetAncestors(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return ExTreeModel.GetDescendantsThatHaveChildren(id);
        }

        protected override bool CanStartDrag(CanStartDragArgs args)
        {
            return true;
        }

        protected override void SetupDragAndDrop(SetupDragAndDropArgs args)
        {
            if (hasSearch)
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where(item => args.draggedItemIDs.Contains(item.id)).ToList();
            DragAndDrop.SetGenericData(k_GenericDragID, draggedRows);
            DragAndDrop.objectReferences = new Object[] { }; // this IS required for dragging to work
            var title = draggedRows.Count == 1 ? draggedRows[0].displayName : "< Multiple >";
            DragAndDrop.StartDrag(title);
        }

        protected override DragAndDropVisualMode HandleDragAndDrop(DragAndDropArgs args)
        {
            // Check if we can handle the current drag data (could be dragged in from other areas/windows in the editor)
            var draggedRows = DragAndDrop.GetGenericData(k_GenericDragID) as List<TreeViewItem>;
            if (draggedRows == null)
                return DragAndDropVisualMode.None;

            // Parent item is null when dragging outside any tree view items.
            switch (args.dragAndDropPosition)
            {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems:
                {
                    var validDrag = ValidDrag(args.parentItem, draggedRows);
                    if (args.performDrop && validDrag)
                    {
                        var parentData = ((TreeViewItem<T>)args.parentItem).data;
                        OnDropDraggedElementsAtIndex(draggedRows, parentData,
                            args.insertAtIndex == -1 ? 0 : args.insertAtIndex);
                    }

                    return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
                }

                case DragAndDropPosition.OutsideItems:
                {
                    if (args.performDrop)
                        OnDropDraggedElementsAtIndex(draggedRows, ExTreeModel.Root, ExTreeModel.Root.Children.Count);

                    return DragAndDropVisualMode.Move;
                }
                default:
                    Debug.LogError("Unhandled enum " + args.dragAndDropPosition);
                    return DragAndDropVisualMode.None;
            }
        }

        public virtual void OnDropDraggedElementsAtIndex(List<TreeViewItem> draggedRows, T parent, int insertIndex)
        {
            if (beforeDroppingDraggedItems != null)
                beforeDroppingDraggedItems(draggedRows);

            var draggedElements = new List<ExTreeElement>();
            foreach (var x in draggedRows)
                draggedElements.Add(((TreeViewItem<T>)x).data);

            var selectedIDs = draggedElements.Select(x => x.ID).ToArray();
            ExTreeModel.MoveElements(parent, insertIndex, draggedElements);
            SetSelection(selectedIDs, TreeViewSelectionOptions.RevealAndFrame);
        }


        private bool ValidDrag(TreeViewItem parent, List<TreeViewItem> draggedItems)
        {
            var currentParent = parent;
            while (currentParent != null)
            {
                if (draggedItems.Contains(currentParent))
                    return false;
                currentParent = currentParent.parent;
            }

            return true;
        }
    }
}
#endif