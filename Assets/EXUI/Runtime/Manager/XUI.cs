using UnityEngine;

namespace EXUI
{
    public static class XUI
    {
        static XUIManager _xUI;
        public static XUIManager M => _xUI; // M for Manager
        
        static EXUIHost _host;

        public static void Launch()
        {
            _xUI = new XUIManager();
            _xUI.LaunchBindingService();
            
            _host = new GameObject("EXUIHost").AddComponent<EXUIHost>();
            Object.DontDestroyOnLoad(_host.gameObject);
            _host.Init(_xUI);
        }
        
        public static void Close()
        {
            Object.DestroyImmediate(_host.gameObject);
            _host = null;
            _xUI = null;
        }
    }
}