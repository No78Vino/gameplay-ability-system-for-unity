using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    public static class GASManager
    {
        public static EntityManager EntityManager { get; private set; }

        //public static World World { get; }

        public static TurnController TurnController { get; private set; }

        public static bool IsRunning { get; private set;}
        
        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized)
            {
#if UNITY_EDITOR
                Debug.Log("EX-GAS has been initialized.Don't reinitialize.");
#endif
                return;
            }


            TurnController ??= new TurnController();
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // 系统逻辑帧计时器
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton<GlobalTimer>();
            IsInitialized = true;
        }

        public static void Run() => IsRunning = true;

        public static void Stop() => IsRunning = false;
    }
}