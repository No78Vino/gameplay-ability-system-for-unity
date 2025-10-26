using GAS.Runtime.System.Attribute;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class GASManager
    {
        public static World ExWorld { get; private set; }
        public static EntityManager EntityManager { get; private set; }

        public static TurnController TurnController { get; private set; }

        public static bool IsRunning { get; private set; }

        public static bool IsInitialized { get; private set; }

        public static Entity EntityGlobalTimer { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized)
            {
#if UNITY_EDITOR
                Debug.LogWarning("EX-GAS has been initialized.Don't reinitialize.");
#endif
                return;
            }


            TurnController ??= new TurnController();
            ExWorld = new World("EX_GAS_World");
            EntityManager = ExWorld.EntityManager;
            CreateSystems();
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

        private static void CreateSystems()
        {
            // 创建基础系统组
            var sgInitialization = ExWorld.CreateSystemManaged<InitializationSystemGroup>();
            var sgSimulation = ExWorld.CreateSystemManaged<SimulationSystemGroup>();
            var sgPresentation = ExWorld.CreateSystemManaged<PresentationSystemGroup>();
            var sgFixedStepSimulation = ExWorld.CreateSystemManaged<FixedStepSimulationSystemGroup>();
            sgFixedStepSimulation.RateManager = new RateUtils.FixedRateSimpleManager(Time.fixedDeltaTime);
            sgSimulation.AddSystemToUpdateList(sgFixedStepSimulation);

            // 创建系统组
            //////////////////////// 逻辑 系统组 //////////////////////////////////
            var sgLogic = ExWorld.CreateSystemManaged<SGLogic>();
            sgFixedStepSimulation.AddSystemToUpdateList(sgLogic);

            var sgAbility = ExWorld.CreateSystemManaged<SGAbility>();
            var sgAttribute = ExWorld.CreateSystemManaged<SysGrpAttribute>();
            var sgEffect = ExWorld.CreateSystemManaged<SGEffect>();
            sgLogic.AddSystemToUpdateList(sgAbility);
            sgLogic.AddSystemToUpdateList(sgAttribute);
            sgLogic.AddSystemToUpdateList(sgEffect);

            var sgLogicTick = ExWorld.CreateSystemManaged<SysGrpLogicTick>();
            sgFixedStepSimulation.AddSystemToUpdateList(sgLogicTick);

            var sgTickAbility = ExWorld.CreateSystemManaged<SysGrpTickAbility>();
            sgLogicTick.AddSystemToUpdateList(sgTickAbility);

            #region Core

            sgLogic.AddSystemToUpdateList(
                ExWorld.CreateSystem<SGlobalTimer>());
            sgLogic.SortSystems();

            #endregion

            #region Ability

            // logic
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryActivateAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryCancelAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryEndAbility>());
            sgAbility.SortSystems();
            // tick
            sgTickAbility.AddSystemToUpdateList(ExWorld.CreateSystem<SAbilityTick>());
            sgTickAbility.SortSystems();

            #endregion

            #region Attribute

            sgAttribute.AddSystemToUpdateList(ExWorld.CreateSystem<SUpdateAttributeCurrentValue>());
            sgAttribute.AddSystemToUpdateList(ExWorld.CreateSystem<SUpdateAttributeBaseValue>());
            sgAttribute.SortSystems();

            #endregion

            #region GameplayEffect

            // 一级：Create->Operation->Destroy->Tick
            var sgEffectCreate = ExWorld.CreateSystemManaged<SGEffectCreate>();
            var sgEffectOperation = ExWorld.CreateSystemManaged<SGEffectOperation>();
            var sgEffectDestroy = ExWorld.CreateSystemManaged<SGEffectDestroy>();
            var sgEffectTick = ExWorld.CreateSystemManaged<SGEffectTick>();
            sgEffect.AddSystemToUpdateList(sgEffectCreate);
            sgEffect.AddSystemToUpdateList(sgEffectOperation);
            sgEffect.AddSystemToUpdateList(sgEffectDestroy);
            sgEffect.AddSystemToUpdateList(sgEffectTick);
            sgEffect.SortSystems();

            // 二级：
            // Create: InstantiateEffect
            var sgInstantiateEffect = ExWorld.CreateSystemManaged<SGInstantiateEffect>();
            sgEffectCreate.AddSystemToUpdateList(sgInstantiateEffect);
            sgEffectCreate.SortSystems();

            // Operation: CheckApply,Apply,CheckActivate,Activate,Deactivate,Remove
            var sgCheckApplyEffect = ExWorld.CreateSystemManaged<SGCheckApplyEffect>();
            var sgApplyEffect = ExWorld.CreateSystemManaged<SGApplyEffect>();
            var sgCheckActivateEffect = ExWorld.CreateSystemManaged<SGCheckActivateEffect>();
            var sgActivateEffect = ExWorld.CreateSystemManaged<SGActivateEffect>();
            var sgDeactivateEffect = ExWorld.CreateSystemManaged<SGDeactivateEffect>();
            var sgRemoveEffect = ExWorld.CreateSystemManaged<SGRemoveEffect>();
            sgEffectOperation.AddSystemToUpdateList(sgCheckApplyEffect);
            sgEffectOperation.AddSystemToUpdateList(sgApplyEffect);
            sgEffectOperation.AddSystemToUpdateList(sgCheckActivateEffect);
            sgEffectOperation.AddSystemToUpdateList(sgActivateEffect);
            sgEffectOperation.AddSystemToUpdateList(sgDeactivateEffect);
            sgEffectOperation.AddSystemToUpdateList(sgRemoveEffect);
            sgEffectOperation.SortSystems();

            // Destroy: KillEffect
            var sgKillEffect = ExWorld.CreateSystemManaged<SGKillEffect>();
            sgEffectDestroy.AddSystemToUpdateList(sgKillEffect);
            sgEffectDestroy.SortSystems();

            // Tick: RunningEffect
            var sgRunningEffect = ExWorld.CreateSystemManaged<SGRunningEffect>();
            sgEffectTick.AddSystemToUpdateList(sgRunningEffect);
            sgEffectTick.SortSystems();

            // 三级：
            // Apply：InstantEffect,DurationalEffect
            var sgInstantEffectApply = ExWorld.CreateSystemManaged<SGInstantEffect>();
            var sgDurationalEffectApply = ExWorld.CreateSystemManaged<SGDurationalEffect>();
            sgApplyEffect.AddSystemToUpdateList(sgInstantEffectApply);
            sgApplyEffect.AddSystemToUpdateList(sgDurationalEffectApply);
            sgApplyEffect.SortSystems();

            // Gameplay Effect 功能系统

            #endregion


            /////////////////////////// 表现 系统组 //////////////////////////////
            var sgDisplay = ExWorld.CreateSystemManaged<SysGrpDisplay>();
            sgSimulation.AddSystemToUpdateList(sgDisplay);
            sgSimulation.SortSystems();

            #region Cue

            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueStart>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueTick>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueEnd>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueDestroy>());
            sgDisplay.SortSystems();

            #endregion
            

            // 将world更新同步PlayerLoop
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(ExWorld);
        }
    }
}