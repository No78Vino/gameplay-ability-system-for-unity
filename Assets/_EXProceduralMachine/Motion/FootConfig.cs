using System;
using UnityEngine;

namespace EXProceduralMachine
{
    [Serializable]
    public struct FootConfig
    {
        public Transform idlePoint;
        public Transform ikTrack;
        public Vector3 offset;
    }
}