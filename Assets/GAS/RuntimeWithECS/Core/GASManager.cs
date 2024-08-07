using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    public static class GASManager
    {
        private static bool _isInitialized;
        private static World _world;
        private static EntityManager _entityManager;
        private static TurnController _turnController;
        private static GASController _gasController;
        
        public static EntityManager EntityManager => _entityManager;
        public static World World => _world;
        public static TurnController TurnController=> _turnController;
        public static GASController GASController => _gasController; 
        
        public static void Initialize()
        {
            if (_isInitialized)
            {
#if UNITY_EDITOR
                Debug.Log("EX-GAS has been initialized.Don't reinitialize.");
#endif
                return;
            }
            
            var goController = new GameObject("EX-GAS Controller");
            Object.DontDestroyOnLoad(goController);
            //goController.hideFlags = HideFlags.HideAndDontSave;
            //_gasController = goController.AddComponent<GASController>();
            _turnController ??= new TurnController();
            //_world ??= new World("EX-GAS");
            //_entityManager = _world.EntityManager;
    
            // 系统逻辑帧计时器
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(GlobalFrameTimer));
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(typeof(GlobalFrameTimer));
            //_entityManager.CreateEntity(typeof(GlobalFrameTimer));
            //_world.CreateSystem<GASTimerSystem>(); 
            //_world.CreateSystem<DebugSystem>(); 
            
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
            _isInitialized = true;
        }
    }
}