using GAS.RuntimeWithECS.Core;
using UnityEngine;

namespace GAS.RuntimeWithECS
{
    public class DebugGASManager : MonoBehaviour
    {
        public bool isGASRunning;

        private void Update()
        {
            if (isGASRunning)
                GASManager.Run();
            else
                GASManager.Stop();
        }
    }
}