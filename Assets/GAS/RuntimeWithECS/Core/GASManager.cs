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
            // 创建基础系统组
            var sgInitialization = ExWorld.CreateSystemManaged<InitializationSystemGroup>();
            var sgSimulation = ExWorld.CreateSystemManaged<SimulationSystemGroup>();
            var sgPresentation = ExWorld.CreateSystemManaged<PresentationSystemGroup>();
            var sgFixedStepSimulation = ExWorld.CreateSystemManaged<FixedStepSimulationSystemGroup>();
            sgFixedStepSimulation.RateManager = new RateUtils.FixedRateSimpleManager(Time.fixedDeltaTime);
            sgSimulation.AddSystemToUpdateList(sgFixedStepSimulation);

            // 创建系统组
            // 逻辑帧 系统组
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
            
            // 表现帧 系统组
            var sgDisplay = ExWorld.CreateSystemManaged<SysGroupDisplay>();
            sgSimulation.AddSystemToUpdateList(sgDisplay);
            sgSimulation.SortSystems();
            
            // 创建系统
            // Core
            sgLogic.AddSystemToUpdateList(
                ExWorld.CreateSystem<SGlobalTimer>());
            sgLogic.SortSystems();
            
            // Ability
            // logic
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryActivateAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryCancelAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryEndAbility>());
            sgAbility.SortSystems();
            // tick
            sgTickAbility.AddSystemToUpdateList(ExWorld.CreateSystem<SAbilityTick>());
            sgTickAbility.SortSystems();

            // Attribute
            sgAttribute.AddSystemToUpdateList(ExWorld.CreateSystem<SUpdateAttributeCurrentValue>());
            sgAttribute.AddSystemToUpdateList(ExWorld.CreateSystem<SUpdateAttributeBaseValue>());
            sgAttribute.SortSystems();
            
            // GameplayEffect
            // logic
            sgEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SApplyGameplayEffect>());
            sgEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SKillGameplayEffect>());
            sgEffect.SortSystems();
            
            // tick
            sgTickGameplayEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectDurationTick>());
            sgTickGameplayEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectPeriodTick>());
            sgTickGameplayEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectStackingTick>());
            sgTickGameplayEffect.SortSystems();
            
            // Cue
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueStart>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueTick>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueEnd>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueDestroy>());
            sgDisplay.SortSystems();
            
            // 将world更新同步PlayerLoop
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(ExWorld);
        }
    }
}