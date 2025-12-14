using UnityEngine;

namespace EXUI
{
    public static class EXUIExtension
    {
        public static Transform Node(this Transform transform, string name)
        {
            return transform.Find(name);
        }

        public static Transform Node(this GameObject gameObject, string name)
        {
            return gameObject.transform.Find(name);
        }

        public static TCom GetComponentByNode<TCom>(this Transform transform, string path) where TCom : Component
        {
            var node = transform.Node(path);
            if (node != null) return transform.Node(path).GetComponent<TCom>();

            Debug.LogError($"Can't Find Node {path}");
            return null;
        }

        public static TCom GetComponentByNode<TCom>(this GameObject gameObject, string path) where TCom : Component
        {
            return gameObject.transform.GetComponentByNode<TCom>(path);
        }
    }
}