using System;

namespace GAS.Runtime
{
    [Serializable]
    public struct AttrInASCustomConfig
    {
        public int AttrCode;
        public bool ClampMin;
        public float ValueMin;
        public bool ClampMax;
        public float ValueMax;
        public bool UseValueInit;
        public float ValueDefaultInit;
    }
}