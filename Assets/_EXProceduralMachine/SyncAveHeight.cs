using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EXProceduralMachine
{
    public class SyncAveHeight : MonoBehaviour
    {
        public Transform body;
        
        public List<Transform> Targets;
        public float offset = 2;
        
        void Update()
        {
            var pos = transform.position;
            pos.y = body.position.y;
            transform.position = pos;
        }
    }
}