using System;
using Unity.Collections;

namespace GAS.RuntimeWithECS.Tag
{
    public struct GASTag
    {
        public readonly int ENUM;
        public readonly NativeArray<int> Parents;
        public readonly NativeArray<int> Children;

        public GASTag(int tagEnum, int[] parents, int[] children)
        {
            ENUM = tagEnum;
            Parents = new NativeArray<int>(parents ?? Array.Empty<int>(), Allocator.Persistent);
            Children = new NativeArray<int>(children ?? Array.Empty<int>(), Allocator.Persistent);
        }
        
        public bool HasTag(int tag)
        {
            if (ENUM == tag) return true;
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
                if (pTag == tag.ENUM)
                    return true;

            return false;
        }

        public bool HasChildTag(GASTag child)
        {
            foreach (var cTag in Children)
                if (cTag == child.ENUM)
                    return true;

            return false;
        }
        
        public bool HasParentTag(GASTag tag)
        {
            foreach (var pTag in Parents)
                if (pTag == tag.ENUM)
                    return true;

            return false;
        }
        
        public static bool operator ==(GASTag x, GASTag y)
        {
            return x.ENUM == y.ENUM;
        }

        public static bool operator !=(GASTag x, GASTag y)
        {
            return x.ENUM != y.ENUM;
        }
        
        public bool IsRoot => Parents.Length == 0;
        public bool HasChild => Children.Length > 0;
    }
}