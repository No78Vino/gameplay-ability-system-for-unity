using System;
using Unity.Collections;

namespace GAS.RuntimeWithECS.Tag
{
    public struct GASTag
    {
        public readonly int Code;
        public readonly int[] Parents;
        public readonly int[] Children;

        public GASTag(int tagCode, int[] parents, int[] children)
        {
            Code = tagCode;
            Parents = parents ?? Array.Empty<int>();
            Children = children ?? Array.Empty<int>();
        }
        
        public bool HasTag(int tag)
        {
            if (Code == tag) return true;
            foreach (var pTag in Parents)
                if (pTag == tag)
                    return true;

            return false;
        }

        public bool HasChildTag(int child)
        {
            foreach (var cTag in Children)
                if (cTag == child)
                    return true;

            return false;
        }
        
        public bool HasParentTag(int tag)
        {
            foreach (var pTag in Parents)
                if (pTag == tag)
                    return true;

            return false;
        }
        
        public bool HasTag(GASTag tag)
        {
            if (this == tag) return true;
            foreach (var pTag in Parents)
                if (pTag == tag.Code)
                    return true;

            return false;
        }

        public bool HasChildTag(GASTag child)
        {
            foreach (var cTag in Children)
                if (cTag == child.Code)
                    return true;

            return false;
        }
        
        public bool HasParentTag(GASTag tag)
        {
            foreach (var pTag in Parents)
                if (pTag == tag.Code)
                    return true;

            return false;
        }
        
        public static bool operator ==(GASTag x, GASTag y)
        {
            return x.Code == y.Code;
        }

        public static bool operator !=(GASTag x, GASTag y)
        {
            return x.Code != y.Code;
        }
        
        public bool IsRoot => Parents.Length == 0;
        public bool HasChild => Children.Length > 0;
    }
}