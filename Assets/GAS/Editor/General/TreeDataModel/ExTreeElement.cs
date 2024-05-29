#if UNITY_EDITOR
namespace UnityEditor.TreeDataModel
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    
    [Serializable]
    public class ExTreeElement
    {
        [SerializeField] private int _id;

        [SerializeField] private string _name;

        [SerializeField] private int _depth;

        [NonSerialized] private List<ExTreeElement> _children;
        [NonSerialized] private ExTreeElement _parent;

        public ExTreeElement()
        {
        }

        public ExTreeElement(string name, int depth, int id)
        {
            _name = name;
            _id = id;
            _depth = depth;
        }

        public int Depth
        {
            get => _depth;
            set => _depth = value;
        }

        public ExTreeElement Parent
        {
            get => _parent;
            set => _parent = value;
        }

        public List<ExTreeElement> Children
        {
            get => _children;
            set => _children = value;
        }

        public bool HasChildren => Children != null && Children.Count > 0;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public int ID
        {
            get => _id;
            set => _id = value;
        }
    }
}
#endif