namespace EXUI
{
    public static class XUI
    {
        static XUIManager _xUI;
        public static XUIManager M => _xUI; // M for Manager

        public static void Launch()
        {
            _xUI = new XUIManager();
            _xUI.Init();
        }
        
        public static void Dispose()
        {
            _xUI.OnDispose();
            _xUI = null;
        }
    }
}