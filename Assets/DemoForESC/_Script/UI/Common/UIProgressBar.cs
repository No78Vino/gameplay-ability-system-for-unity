using EXUI;
using Loxodon.Framework.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace DemoForESC
{
    public class UIProgressBar
    {
        private Transform _node;
        public Text LabelValue { get; private set; }
        public Text LabelName { get; private set; }
        public Image ValueBar { get; private set; }


        public UIProgressBar(GameObject gameObject)
        {
            _node = gameObject.transform;
            LabelValue = _node.GetComponentByNode<Text>("label_value");
            LabelName = _node.GetComponentByNode<Text>("label_name");
            ValueBar = _node.GetComponentByNode<Image>("value");
        }

        public UIProgressBar(Transform transform)
        {
            _node = transform;
            LabelValue = _node.GetComponentByNode<Text>("label_value");
            LabelName = _node.GetComponentByNode<Text>("label_name");
            ValueBar = _node.GetComponentByNode<Image>("value");
        }
    }
}