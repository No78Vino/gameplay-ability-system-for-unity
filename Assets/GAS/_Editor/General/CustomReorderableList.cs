using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GAS.Editor.General
{
    public class CustomReorderableList<T>
    {
        private List<T> _itemList = new List<T>();
        private ReorderableList reorderableList;
        private Action<int, T> _onEdit;
        private Action<Rect,T,int> _itemGUIDraw;
        private ReorderableList.ElementHeightCallbackDelegate _getElementHeight;
        
        public CustomReorderableList(
            List<T> itemList ,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight)
        {
            _itemList = itemList;
            _onEdit = onEdit;
            _itemGUIDraw = itemGUIDraw;
            _getElementHeight = getElementHeight;
            OnEnable();
        }
        
        public CustomReorderableList(
            T[] itemList ,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight)
        {
            _itemList = new List<T>();
            foreach (var t in itemList)
            {
                _itemList.Add(t);
            }
            _onEdit = onEdit;
            _itemGUIDraw = itemGUIDraw;
            _getElementHeight = getElementHeight;
            OnEnable();
        }
        
        void OnEnable()
        {
            reorderableList = new ReorderableList(_itemList, typeof(T), true, true, true, true);
            reorderableList.drawElementCallback += DrawListElement;
            reorderableList.drawHeaderCallback += DrawListHeader;
            if(_getElementHeight != null) reorderableList.elementHeightCallback += _getElementHeight;
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

            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical(GUILayout.Width(300));
            _itemGUIDraw?.Invoke(rect,element,index);
            EditorGUILayout.EndVertical();

            if (_onEdit != null)
            {
                if (GUI.Button(new Rect(rect.x + rect.width - 60, rect.y, 60, EditorGUIUtility.singleLineHeight),
                        "Edit"))
                    ShowEditWindow(index);
            }

            EditorGUILayout.EndHorizontal();
        }
        
        private void AddListItem(ReorderableList list)
        {
            _itemList.Add(default);
        }

        private void ShowEditWindow(int index)
        {
            _onEdit.Invoke(index,_itemList[index]);
        }

        public static CustomReorderableList<T> Create(
            List<T> itemList ,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight = null)
        {
            return new CustomReorderableList<T>(itemList,onEdit,itemGUIDraw,getElementHeight);
        }
        
        public static CustomReorderableList<T> Create(
            T[] itemList ,
            Action<int,T> onEdit,
            Action<Rect,T,int> itemGUIDraw,
            ReorderableList.ElementHeightCallbackDelegate getElementHeight = null)
        {
            return new CustomReorderableList<T>(itemList,onEdit,itemGUIDraw,getElementHeight);
        }
        
        public void UpdateItem(int index, T item)
        {
            _itemList[index] = item;
        }
    }
}