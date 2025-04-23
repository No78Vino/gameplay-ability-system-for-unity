using GAS.RuntimeWithECS.System.Attribute;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class GASManager
    {
        public static World ExWorld { get; private set; }
        public static EntityManager EntityManager { get; private set; }

        //public static World World { get; }

        public static TurnController TurnController { get; private set; }

        public static bool IsRunning { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static Entity EntityGlobalTimer { get; private set; }

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
            ExWorld = new World("EX_GAS_World");
            EntityManager = ExWorld.EntityManager;
            CreateGasSystems();
            // 系统逻辑帧计时器
            EntityGlobalTimer = ExWorld.EntityManager.CreateSingleton<GlobalTimer>();
            IsInitialized = true;
        }

        public static void Run()
        {
            IsRunning = true;
        }

        public static void Stop()
        {
            IsRunning = false;
        }

        private static void CreateGasSystems()
        {
            ExWorld.CreateSystemManaged<InitializationSystemGroup>();
            var sss = ExWorld.CreateSystemManaged<SimulationSystemGroup>();
            ExWorld.CreateSystemManaged<PresentationSystemGroup>();
            
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(ExWorld);
            
            var testGroup = ExWorld.GetOrCreateSystemManaged<SysGroupTest>();
            var test = ExWorld.GetOrCreateSystem<STestSystem>();
            sss.AddSystemToUpdateList(test);

            // Create the system groups
      
            ExWorld.CreateSystemManaged<FixedStepSimulationSystemGroup>();
            
            ExWorld.CreateSystemManaged<SysGroupLogic>();
            ExWorld.CreateSystemManaged<SysGroupAbility>();
            ExWorld.CreateSystemManaged<SysGroupAttribute>();
            ExWorld.CreateSystemManaged<SysGroupEffect>();
            
            ExWorld.CreateSystemManaged<SysGroupLogicTick>();
            var s = ExWorld.CreateSystemManaged<SysGroupTickAbility>();
            

            ExWorld.CreateSystemManaged<SysGroupTickGameplayEffect>();
            
            // Create the systems
            
            // Core
            ExWorld.CreateSystem<SGlobalTimer>();
            
            // Ability
            ExWorld.CreateSystem<SAbilityTick>();
            ExWorld.CreateSystem<STryActivateAbility>();
            ExWorld.CreateSystem<STryCancelAbility>();
            ExWorld.CreateSystem<STryEndAbility>();
            
            // Attribute
            ExWorld.CreateSystem<SUpdateAttributeCurrentValue>();
            ExWorld.CreateSystem<SUpdateAttributeBaseValue>();
        }

        public static void FixedUpdate()
        {
            ExWorld.Update();
        }
    }
}