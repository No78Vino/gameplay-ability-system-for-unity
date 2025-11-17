using UnityEngine;

namespace EXUI
{
    public class EXUIHost : MonoBehaviour
    {
        private XUIManager _xuiManager;

        public void Init(XUIManager xuiManager)
        {
            _xuiManager = xuiManager;
        }
        
        private void Update()
        {
            _xuiManager?.UITick();
        }

        private void OnDestroy()
        {
            _xuiManager?.OnDispose();
        }
    }
}