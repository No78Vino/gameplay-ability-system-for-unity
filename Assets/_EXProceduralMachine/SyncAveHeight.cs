using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EXProceduralMachine
{
    public class SyncAveHeight : MonoBehaviour
    {
        public List<Transform> Targets;
        public float offset = 2;
        
        void Update()
        {
            var pos = transform.position;
            float ave = Targets.Sum(target => target.position.y);
            ave /= Targets.Count;
            transform.position = new Vector3(pos.x, ave + offset, pos.z);
        }
    }
}