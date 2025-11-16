using UnityEngine;

namespace EXMaidForUI.Runtime.EXMaid
{
    public class EXMaidUIHost : MonoBehaviour
    {
        private IEXMaidUI _exMaidUI;

        public void Init(IEXMaidUI exMaidUI)
        {
            _exMaidUI = exMaidUI;
        }
        
        private void Update()
        {
            _exMaidUI?.UITick();
        }

        private void OnDestroy()
        {
            _exMaidUI?.OnDispose();
        }
    }
}