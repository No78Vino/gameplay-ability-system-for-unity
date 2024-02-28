using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class ListViewExampleWindow : EditorWindow
{
    [MenuItem("Window/ListViewExampleWindow")]
    public static void OpenDemoManual()
    {
        GetWindow<ListViewExampleWindow>().Show();
    }

    public void OnEnable()
    {
        // Create a list of data. In this case, numbers from 1 to 1000.
        const int itemCount = 1000;
        var items = new List<string>(itemCount);
        for (int i = 0; i <= itemCount - 1; i++)
            items.Add(i.ToString());

        // The "makeItem" function is called when the
        // ListView needs more items to render.
        Func<VisualElement> makeItem = () => new Label();

        // As the user scrolls through the list, the ListView object
        // recycles elements created by the "makeItem" function,
        // and invoke the "bindItem" callback to associate
        // the element with the matching data item (specified as an index in the list).
        Action<VisualElement, int> bindItem = (e, i) => (e as Label).text = items[i];

        // Provide the list view with an explicit height for every row
        // so it can calculate how many items to actually display
        const int itemHeight = 16;

        ListView listView = new ListView(items, itemHeight, makeItem, bindItem)
        {
            // Enables multiple selection using shift or ctrl/cmd keys.
            selectionType = SelectionType.Multiple
        };

        // Set up list view so that you can add or remove items dynamically.
        listView.showAddRemoveFooter = true;

        // Implement functionality on the list view to add or remove items.
        // listView.onAdd = view =>
        // {
        //     var itemsSourceCount = view.itemsSource.Count;
        //     view.itemsSource.Add(itemsSourceCount.ToString());
        //     view.RefreshItems();
        //     view.ScrollToItem(itemsSourceCount);
        // };
        
        // listView.onRemove = view =>
        // {
        //     var itemsSourceCount = view.itemsSource.Count;
        //     view.itemsSource.RemoveAt(itemsSourceCount - 1);
        //     view.RefreshItems();
        //     view.ScrollToItem(itemsSourceCount - 2);
        // };

        // Single click triggers "selectionChanged" with the selected items. (f.k.a. "onSelectionChange")
        // Use "selectedIndicesChanged" to get the indices of the selected items instead. (f.k.a. "onSelectedIndicesChange")
        listView.selectionChanged += objects => Debug.Log($"Selected: {string.Join(", ", objects)}");

        // Double-click triggers "itemsChosen" with the selected items. (f.k.a. "onItemsChosen")
        listView.itemsChosen += objects => Debug.Log($"Double-clicked: {string.Join(", ", objects)}");

        listView.style.flexGrow = 1.0f;

        rootVisualElement.Add(listView);

        // Allow to add items, but only when there is no override, by using a Toggle.
        //listView.allowAdd = true;
        var toggle = new Toggle("Override Add functionality")
        {
            value = false, //listView.overridingAddButtonBehavior != null,
            style = {alignSelf = Align.Auto},
        };
        
        // toggle.RegisterValueChangedCallback(evt => listView.overridingAddButtonBehavior = evt.newValue
        //     ? (view, button) =>
        //     {
        //         Debug.Log("You cannot add new items at this time");
        //     }
        //     : null
        // );
        rootVisualElement.Add(toggle);
    }
}