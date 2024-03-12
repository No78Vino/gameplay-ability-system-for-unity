#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;
    
    public class CustomReorderableList<T>
    {
        private List<T> _itemList = new List<T>();
        private ReorderableList reorderableList;
        private ReorderableList.HeaderCallbackDelegate _drawListHeader;
        private Action<int, T> _onEdit;
        private Action<Rect,T,int> _itemGUIDraw;
        private ReorderableList.ElementHeightCallbackDelegate _getElementHeight;
        ReorderableList.AddCallbackDelegate _onAddListItem;
        public CustomReorderableList(
            List<T> itemList ,
            ReorderableList.HeaderCallbackDelegate drawListHeader,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight,
            ReorderableList.AddCallbackDelegate onAddListItem)
        {
            _itemList = itemList;
            _onEdit = onEdit;
            _itemGUIDraw = itemGUIDraw;
            _getElementHeight = getElementHeight;
            _onAddListItem = onAddListItem;
            _drawListHeader = drawListHeader;
            OnEnable();
        }
        
        public CustomReorderableList(
            T[] itemList ,
            ReorderableList.HeaderCallbackDelegate drawListHeader,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight,
            ReorderableList.AddCallbackDelegate onAddListItem)
        {
            _itemList = new List<T>();
            foreach (var t in itemList)
            {
                _itemList.Add(t);
            }
            _onEdit = onEdit;
            _itemGUIDraw = itemGUIDraw;
            _getElementHeight = getElementHeight;
            _onAddListItem = onAddListItem;
            _drawListHeader = drawListHeader;
            OnEnable();
        }

        void OnEnable()
        {
            reorderableList = new ReorderableList(_itemList, typeof(T), true, true, true, true);

            if (_drawListHeader != null)
                reorderableList.drawHeaderCallback += _drawListHeader;
            else
                reorderableList.drawHeaderCallback += DrawListHeader;

            reorderableList.drawElementCallback += DrawListElement;

            if (_getElementHeight != null) reorderableList.elementHeightCallback += _getElementHeight;
            
            reorderableList.onAddCallback += AddListItem;
        }

        public void OnGUI()
        {
            reorderableList.DoLayoutList();
        }

        private void DrawListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, $"{typeof(T).Name} List");
        }

        private void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = _itemList[index];
            rect.y += 2;
            
            _itemGUIDraw?.Invoke(rect,element,index);
            
            if (_onEdit != null)
            {
                if (GUI.Button(new Rect(rect.x + rect.width - 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
                        "Edit"))
                    ShowEditWindow(index);
            }
        }
        
        private void AddListItem(ReorderableList list)
        {
            _itemList.Add(default);
            _onAddListItem?.Invoke(list);
        }

        private void ShowEditWindow(int index)
        {
            _onEdit.Invoke(index,_itemList[index]);
        }

        public static CustomReorderableList<T> Create(
            List<T> itemList ,
            ReorderableList.HeaderCallbackDelegate drawListHeader,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight,
            ReorderableList.AddCallbackDelegate onAddListItem)
        {
            return new CustomReorderableList<T>(itemList,drawListHeader,onEdit,itemGUIDraw,getElementHeight,onAddListItem);
        }
       
        public static CustomReorderableList<T> Create(
            T[] itemList ,
            ReorderableList.HeaderCallbackDelegate drawListHeader,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight,
            ReorderableList.AddCallbackDelegate onAddListItem)
        {
            itemList = itemList ?? new T[0];
            return new CustomReorderableList<T>(itemList,drawListHeader,onEdit,itemGUIDraw,getElementHeight,onAddListItem);
        }
        
        public void UpdateItem(int index, T item)
        {
            _itemList[index] = item;
        }
        
        public List<T> GetItemList()
        {
            return _itemList;
        }
    }
}
#endif