using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GAS.Editor.Ability
{
    public static class TrackInspectorUtil
    {
        private const int LineHeight = 25;
        private const int ButtonHeight = 35;
        private const int LabelWidth = 35;
        
        public static VisualElement CreateTrackInspector()
        {
            var inspector = new VisualElement();
            inspector.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            inspector.style.height = new StyleLength(new Length(100,LengthUnit.Percent));
            inspector.style.alignItems = Align.Center;
            inspector.style.alignContent = Align.Center;
            inspector.style.paddingLeft = 3;
            inspector.style.paddingRight = 3;
            inspector.style.paddingTop = 6;
            inspector.style.paddingBottom = 3;
            return inspector;
        }
        
        public static TextField CreateTextField(string label,string initValue,EventCallback<ChangeEvent<string>> onValueChanged)
        {
            var textField = new TextField(label);
            textField.isDelayed = true;
            textField.value = initValue;
            textField.RegisterValueChangedCallback(onValueChanged);
            textField.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            textField.style.height = LineHeight;
            textField.style.alignSelf = Align.Auto;
            textField.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            textField.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            textField.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            return textField;
        }

        public static Label CreateLabel(string label)
        {
            var textLabel = new Label(label);
            textLabel.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            textLabel.style.height = LineHeight;
            return textLabel;
        }
        
        public static ObjectField CreateObjectField(string label, System.Type type, UnityEngine.Object initValue, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        {
            var objectField = new ObjectField(label);
            objectField.objectType = type;
            objectField.value = initValue;
            objectField.RegisterValueChangedCallback(onValueChanged);
            objectField.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            objectField.style.height = LineHeight;
            objectField.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            objectField.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            objectField.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            
            return objectField;
        }
        
        public static IntegerField CreateIntegerField(string label,int initValue,EventCallback<ChangeEvent<int>> onValueChanged)
        {
            var integerField = new IntegerField(label);
            integerField.isDelayed = true;
            integerField.value = initValue;
            integerField.RegisterValueChangedCallback(onValueChanged);
            integerField.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            integerField.style.height = LineHeight;
            integerField.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            integerField.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            integerField.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            return integerField;
        }
        
        public static FloatField CreateFloatField(string label,float initValue,EventCallback<ChangeEvent<float>> onValueChanged)
        {
            var floatField = new FloatField(label);
            floatField.isDelayed = true;
            floatField.value = initValue;
            floatField.RegisterValueChangedCallback(onValueChanged);
            floatField.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            floatField.style.height = LineHeight;
            floatField.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            floatField.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            floatField.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            return floatField;
        }
        
        public static EnumField CreateEnumField<T>(string label,T initValue,EventCallback<ChangeEvent<Enum>> onValueChanged) where T : Enum
        {
            var enumField = new EnumField(label,initValue);
            enumField.RegisterValueChangedCallback(onValueChanged);
            enumField.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            enumField.style.height = LineHeight;
            enumField.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            enumField.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            enumField.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            return enumField;
        }
        
        public static LayerMaskField CreateLayerMaskField(string label,LayerMask initValue,EventCallback<ChangeEvent<int>> onValueChanged)
        {
            var layerMaskField = new LayerMaskField(label,initValue);
            layerMaskField.RegisterValueChangedCallback(onValueChanged);
            layerMaskField.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            layerMaskField.style.height = LineHeight;
            layerMaskField.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            layerMaskField.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            layerMaskField.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            return layerMaskField;
        }
        
        public static Vector3Field CreateVector3Field(string label,Vector3 initValue,EventCallback<ChangeEvent<Vector3>> onValueChanged)
        {
            var vector3Field = new Vector3Field(label);
            vector3Field.value = initValue;
            vector3Field.RegisterValueChangedCallback(onValueChanged);
            vector3Field.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            vector3Field.style.height = LineHeight;
            vector3Field.labelElement.style.width = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            vector3Field.labelElement.style.minWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            vector3Field.labelElement.style.maxWidth = new StyleLength(new Length(LabelWidth,LengthUnit.Percent));
            return vector3Field;
        }

        public static Button CreateButton(string label,Action onClick)
        {
            var button = new Button(onClick);
            button.text = label;
            button.style.width = new StyleLength(new Length(100,LengthUnit.Percent));
            button.style.height = ButtonHeight;
            return button;
        }

        public static ListView CreateObjectListView<T>(string label, List<T> list,
            Action<int,ChangeEvent<UnityEngine.Object>> onItemValueChanged) where T : UnityEngine.Object
        {
            VisualElement MakeItem()
            {
                var objectField = new ObjectField();
                objectField.objectType = typeof(T);
                return objectField;
            }

            void BindItem(VisualElement e, int i)
            {
                var objectField = (ObjectField)e;
                objectField.value = list[i];
                objectField.RegisterValueChangedCallback(evt =>
                {
                    onItemValueChanged(i, evt);
                });
            }

            var listView = new ListView(list,LineHeight,MakeItem,BindItem);
            listView.headerTitle = label;
            listView.reorderable= true;
            listView.showAddRemoveFooter = true;
            listView.showBorder= true;
            listView.showFoldoutHeader = true;
            listView.itemsSource = list;
            listView.selectionType = SelectionType.Single;
            
            listView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            listView.style.height = new StyleLength(StyleKeyword.Auto);
            
            listView.style.paddingLeft = 5;
            listView.style.paddingRight = 5;
            listView.style.paddingTop = 5;
            listView.style.paddingBottom = 5;
            
            listView.style.alignItems = Align.Stretch;
            listView.style.alignContent = Align.Stretch;
            listView.style.justifyContent = Justify.FlexStart;
            listView.style.flexDirection = FlexDirection.Column;
            listView.style.flexWrap = Wrap.NoWrap;
   
            return listView;
        }

        public static ListView CreateStringListView(string label, List<string> list,
            Action<int,ChangeEvent<string>> onItemValueChanged)
        {
            VisualElement MakeItem()
            {
                var textField = new TextField();
                textField.isDelayed = true;
                return textField;
            }

            void BindItem(VisualElement e, int i)
            {
                var textField = (TextField)e;
                textField.value = list[i];
                textField.RegisterValueChangedCallback(evt =>
                {
                    onItemValueChanged(i, evt);
                });
            }

            var listView = new ListView(list,LineHeight,MakeItem,BindItem);
            listView.headerTitle = label;
            listView.reorderable= true;
            listView.showAddRemoveFooter = true;
            listView.showBorder= true;
            listView.showFoldoutHeader = true;
            listView.itemsSource = list;
            listView.selectionType = SelectionType.Single;
            
            listView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            listView.style.height = new StyleLength(StyleKeyword.Auto);
            
            listView.style.paddingLeft = 5;
            listView.style.paddingRight = 5;
            listView.style.paddingTop = 5;
            listView.style.paddingBottom = 5;
            
            listView.style.alignItems = Align.Stretch;
            listView.style.alignContent = Align.Stretch;
            listView.style.justifyContent = Justify.FlexStart;
            listView.style.flexDirection = FlexDirection.Column;
            listView.style.flexWrap = Wrap.NoWrap;
   
            return listView;
        }
        
        
        public static ListView CreateTypeListView(string label, List<Type> list,
            Action<int,ChangeEvent<Type>> onItemValueChanged)
        {
            VisualElement MakeItem()
            {
                var typeField = new ObjectField();//TypeField();
                return typeField;
            }

            void BindItem(VisualElement e, int i)
            {
                var typeField = (ObjectField)e;
                // typeField.value = list[i];
                // typeField.RegisterValueChangedCallback(evt =>
                // {
                //     onItemValueChanged(i, evt);
                // });
            }

            var listView = new ListView(list,LineHeight,MakeItem,BindItem);
            listView.headerTitle = label;
            listView.reorderable= true;
            listView.showAddRemoveFooter = true;
            listView.showBorder= true;
            listView.showFoldoutHeader = true;
            listView.itemsSource = list;
            listView.selectionType = SelectionType.Single;
            
            listView.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            listView.style.height = new StyleLength(StyleKeyword.Auto);
            
            listView.style.paddingLeft = 5;
            listView.style.paddingRight = 5;
            listView.style.paddingTop = 5;
            listView.style.paddingBottom = 5;
            
            listView.style.alignItems = Align.Stretch;
            listView.style.alignContent = Align.Stretch;
            listView.style.justifyContent = Justify.FlexStart;
            listView.style.flexDirection = FlexDirection.Column;
            listView.style.flexWrap = Wrap.NoWrap;
   
            return listView;
        }
    }
}