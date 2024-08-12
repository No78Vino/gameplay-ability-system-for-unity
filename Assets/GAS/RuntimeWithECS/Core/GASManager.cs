using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    public static class GASManager
    {
        private static bool _isInitialized;
        public static EntityManager EntityManager { get; }

        public static World World { get; }

        public static TurnController TurnController { get; private set; }

        public static bool IsRunning { get; private set;}

        public static void Initialize()
        {
            if (_isInitialized)
            {
#if UNITY_EDITOR
                Debug.Log("EX-GAS has been initialized.Don't reinitialize.");
#endif
                return;
            }


            TurnController ??= new TurnController();
            // 系统逻辑帧计时器
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(GlobalFrameTimer));
            _isInitialized = true;
        }

        public static void Run() => IsRunning = true;

        public static void Stop() => IsRunning = false;
    }
}