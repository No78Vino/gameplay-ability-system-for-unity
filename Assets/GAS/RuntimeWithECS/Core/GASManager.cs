using Unity.Entities;
using UnityEngine;

namespace GAS.RuntimeWithECS.Core
{
    public static class GASManager
    {
        private static bool _isInitialized;
        private static World _world;
        private static EntityManager _entityManager;
        
        public static EntityManager EntityManager => _entityManager;
        public static World World => _world;
        
        
        public static void Initialize()
        {
            if (_isInitialized)
            {
                
#if UNITY_EDITOR
                Debug.Log("EX-GAS has been initialized.Don't reinitialize.");
#endif
                return;
            }
            
            _world ??= new World("EX-GAS");
            _entityManager = _world.EntityManager;
            
            _world.CreateSystem<GASTimerSystem>(); // 系统计时器
            
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(_world);
            _isInitialized = true;
        }
    }
}