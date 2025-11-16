using EXMaidForUI.Runtime.FairyGUIExtension;
using UnityEngine;

namespace EXMaidForUI.Runtime.EXMaid
{
    public static class XUI
    {
        static IEXMaidUI _exMaidUI;
        public static IEXMaidUI M => _exMaidUI; // M for Maid
        
        static EXMaidUIHost _exMaidUIHost;

        public static void Launch(string prefix,FairyGUIPackageExtension.OnLoadResource onLoadResourceHandler)
        {
            _exMaidUI = new EXMaidUI();
            _exMaidUI.LaunchBindingService(prefix,onLoadResourceHandler);
            
            _exMaidUIHost = new GameObject("EXMaidUIHost").AddComponent<EXMaidUIHost>();
            _exMaidUIHost.gameObject.hideFlags = HideFlags.HideInHierarchy;
            _exMaidUIHost.Init(_exMaidUI);
        }
        
        public static void Close()
        {
            Object.DestroyImmediate(_exMaidUIHost.gameObject);
            _exMaidUIHost = null;
            _exMaidUI = null;
        }
    }
}