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
            var sgInitialization = ExWorld.CreateSystemManaged<InitializationSystemGroup>();
            var sgSimulation = ExWorld.CreateSystemManaged<SimulationSystemGroup>();
            var sgPresentation = ExWorld.CreateSystemManaged<PresentationSystemGroup>();
            var sgFixedStepSimulation = ExWorld.CreateSystemManaged<FixedStepSimulationSystemGroup>();
            sgSimulation.AddSystemToUpdateList(sgFixedStepSimulation);

            // Create the system groups
            var sgLogic = ExWorld.CreateSystemManaged<SysGroupLogic>();
            sgFixedStepSimulation.AddSystemToUpdateList(sgLogic);

            var sgAbility = ExWorld.CreateSystemManaged<SysGroupAbility>();
            var sgAttribute = ExWorld.CreateSystemManaged<SysGroupAttribute>();
            var sgEffect = ExWorld.CreateSystemManaged<SysGroupEffect>();
            sgLogic.AddSystemToUpdateList(sgAbility);
            sgLogic.AddSystemToUpdateList(sgAttribute);
            sgLogic.AddSystemToUpdateList(sgEffect);

            var sgLogicTick = ExWorld.CreateSystemManaged<SysGroupLogicTick>();
            sgFixedStepSimulation.AddSystemToUpdateList(sgLogicTick);

            var sgTickAbility = ExWorld.CreateSystemManaged<SysGroupTickAbility>();
            var sgTickGameplayEffect = ExWorld.CreateSystemManaged<SysGroupTickGameplayEffect>();
            sgLogicTick.AddSystemToUpdateList(sgTickAbility);
            sgLogicTick.AddSystemToUpdateList(sgTickGameplayEffect);


            // Create the systems

            // Core
            sgLogic.AddSystemToUpdateList(
                ExWorld.CreateSystem<SGlobalTimer>());
            
            // Ability
            ExWorld.CreateSystem<SAbilityTick>();
            ExWorld.CreateSystem<STryActivateAbility>();
            ExWorld.CreateSystem<STryCancelAbility>();
            ExWorld.CreateSystem<STryEndAbility>();

            // Attribute
            ExWorld.CreateSystem<SUpdateAttributeCurrentValue>();
            ExWorld.CreateSystem<SUpdateAttributeBaseValue>();



            // 将world更新同步PlayerLoop
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(ExWorld);
        }
    }
}