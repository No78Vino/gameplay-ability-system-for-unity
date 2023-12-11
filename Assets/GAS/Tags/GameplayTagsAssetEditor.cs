using System.Collections.Generic;
using GAS.Runtime.Tags;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;

namespace GAS.Editor.Tags
{
	// [CustomEditor (typeof(GameplayTagsScriptableObject))]
	public class GameplayTagsAssetEditor : OdinEditorWindow
	{
		GameplayTagsTreeView _treeView;
		SearchField _searchField;
		const string kSessionStateKeyPrefix = "TVS";

		[LabelText("Gameplay Tags Asset")]
		public GameplayTagsScriptableObject asset;
		
		//GameplayTagsScriptableObject asset => (GameplayTagsScriptableObject) target;

		
		[MenuItem("GAS/Gameplay Tags Asset Editor")]
		public static void Open()
		{
			var window = GetWindow<GameplayTagsAssetEditor>("Gameplay Tags Manager");
			window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);
		}
		
		void OnEnable ()
		{
			Undo.undoRedoPerformed += OnUndoRedoPerformed;

			var treeViewState = new TreeViewState ();
			var jsonState = SessionState.GetString (kSessionStateKeyPrefix + asset.GetInstanceID (), "");
			if (!string.IsNullOrEmpty (jsonState))
				JsonUtility.FromJsonOverwrite (jsonState, treeViewState);
			
			List<GameplayTagEditorElement> elements = new List<GameplayTagEditorElement>();
			foreach (var tag in asset.Tags)
			{
				elements.Add(new GameplayTagEditorElement(tag,tag.AncestorHashCodes.Length,_treeView.treeModel.GenerateUniqueID ()));
			}
			var treeModel = new TreeModel<GameplayTagEditorElement> (elements);
			
			//_treeView.treeModel.AddElement(element, parent, 0);
			
			_treeView = new GameplayTagsTreeView(treeViewState, treeModel);
			_treeView.beforeDroppingDraggedItems += OnBeforeDroppingDraggedItems;
			_treeView.Reload ();

			_searchField = new SearchField ();
			
			_searchField.downOrUpArrowKeyPressed += _treeView.SetFocusAndEnsureSelectedItem;
		}


		void OnDisable ()
		{
			Undo.undoRedoPerformed -= OnUndoRedoPerformed;

			SessionState.SetString (kSessionStateKeyPrefix + asset.GetInstanceID (), JsonUtility.ToJson (_treeView.state));
		}

		void OnUndoRedoPerformed ()
		{
			// if (_treeView != null)
			// {
			// 	_treeView.treeModel.SetData (asset.Tags);
			// 	_treeView.Reload ();
			// }
		}

		void OnBeforeDroppingDraggedItems (IList<TreeViewItem> draggedRows)
		{
			Undo.RecordObject (asset, string.Format ("Moving {0} Item{1}", draggedRows.Count, draggedRows.Count > 1 ? "s" : ""));
		}

		public void OnGUI ()
		{
			asset = (GameplayTagsScriptableObject) EditorGUILayout.ObjectField("Gameplay Tags Asset", asset, typeof(GameplayTagsScriptableObject), false);
			GUILayout.Space (5f);
			ToolBar ();
			GUILayout.Space (3f);

			const float topToolbarHeight = 20f;
			const float spacing = 2f;
			float totalHeight = _treeView.totalHeight + topToolbarHeight + 2 * spacing;
			Rect rect = GUILayoutUtility.GetRect (0, 10000, 0, totalHeight);
			Rect toolbarRect = new Rect (rect.x, rect.y, rect.width, topToolbarHeight);
			Rect multiColumnTreeViewRect = new Rect (rect.x, rect.y + topToolbarHeight + spacing, rect.width, rect.height - topToolbarHeight - 2 * spacing);
			SearchBar (toolbarRect);
			DoTreeView (multiColumnTreeViewRect);
		}

		void SearchBar (Rect rect)
		{
			_treeView.searchString = _searchField.OnGUI(rect, _treeView.searchString);
		}

		void DoTreeView (Rect rect)
		{
			_treeView.OnGUI (rect);
		}

		void ToolBar ()
		{
			using (new EditorGUILayout.HorizontalScope ())
			{
				var style = "miniButton";
				if (GUILayout.Button ("Expand All", style))
				{
					_treeView.ExpandAll ();
				}

				if (GUILayout.Button ("Collapse All", style))
				{
					_treeView.CollapseAll ();
				}

				GUILayout.FlexibleSpace ();

				if (GUILayout.Button ("Add Tag", style))
				{
					Undo.RecordObject (asset, "Add Item To Asset");

					// // Add item as child of selection
					// var selection = _treeView.GetSelection ();
					// TreeElement parent = (selection.Count == 1 ? _treeView.treeModel.Find (selection[0]) : null) ?? _treeView.treeModel.root;
					// int depth = parent != null ? parent.depth + 1 : 0;
					// int id = _treeView.treeModel.GenerateUniqueID ();
					//
					// var element = new GameplayTag ("Item " + id, depth, id);
					// _treeView.treeModel.AddElement(element, parent, 0);
					//
					// // Select newly created element
					// _treeView.SetSelection (new[] {id}, TreeViewSelectionOptions.RevealAndFrame);
				}

				if (GUILayout.Button ("Remove Tag", style))
				{
					bool result = EditorUtility.DisplayDialog("Confirmation", "Are you sure you want to REMOVE this tag?", "Yes", "No");

					if (result)
					{
						Undo.RecordObject (asset, "Remove Item From Asset");
						var selection = _treeView.GetSelection ();
						_treeView.treeModel.RemoveElements (selection);
					}
				}
			}
		}
	}
}
