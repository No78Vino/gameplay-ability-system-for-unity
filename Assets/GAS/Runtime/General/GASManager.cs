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
            ExWorld.CreateSystemManaged<InitializationSystemGroup>();
            var sgSimulation = ExWorld.CreateSystemManaged<SimulationSystemGroup>();
            ExWorld.CreateSystemManaged<PresentationSystemGroup>();
            var sgFixedStepSimulation = ExWorld.CreateSystemManaged<FixedStepSimulationSystemGroup>();
            sgFixedStepSimulation.RateManager = new RateUtils.FixedRateSimpleManager(Time.fixedDeltaTime);
            sgSimulation.AddSystemToUpdateList(sgFixedStepSimulation);
            
            //////////////////////// 逻辑 系统组 //////////////////////////////////
            var sgLogic = ExWorld.CreateSystemManaged<SGLogic>();
            sgFixedStepSimulation.AddSystemToUpdateList(sgLogic);

            var sgAbility = ExWorld.CreateSystemManaged<SGAbility>();
            var sgAttribute = ExWorld.CreateSystemManaged<SGAttribute>();
            var sgEffect = ExWorld.CreateSystemManaged<SGEffect>();
            sgLogic.AddSystemToUpdateList(ExWorld.CreateSystem<SGlobalTimer>());
            sgLogic.AddSystemToUpdateList(sgAbility);
            sgLogic.AddSystemToUpdateList(sgAttribute);
            sgLogic.AddSystemToUpdateList(sgEffect);
            sgLogic.SortSystems();

            #region Ability

            // logic
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryActivateAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryCancelAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<STryEndAbility>());
            sgAbility.AddSystemToUpdateList(ExWorld.CreateSystem<SAbilityTick>());
            sgAbility.SortSystems();

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
            sgInstantiateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SInstantiateEffect>());
            sgInstantiateEffect.SortSystems();
            sgEffectCreate.AddSystemToUpdateList(sgInstantiateEffect);
            sgEffectCreate.SortSystems();

            // Operation: CheckApply,Apply,CheckActivate,Activate,Deactivate,Remove
            var sgCheckApplyEffect = ExWorld.CreateSystemManaged<SGCheckApplyEffect>();
            sgCheckApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SCheckApplicationCondition>());
            sgCheckApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SCheckApplicationRequiredTags>());
            sgCheckApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SCheckImmunityTags>());
            sgCheckApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SCheckApplyEnd>());
            sgCheckApplyEffect.SortSystems();

            var sgApplyEffect = ExWorld.CreateSystemManaged<SGApplyEffect>();
            sgApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SPlayCueOnApply>());
            sgApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SRemoveEffectWithTags>());
            sgApplyEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SApplyEnd>());
            sgApplyEffect.SortSystems();

            var sgCheckActivateEffect = ExWorld.CreateSystemManaged<SGCheckActivateEffect>();
            sgCheckActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SCheckEffectActive>());
            sgCheckActivateEffect.SortSystems();

            var sgActivateEffect = ExWorld.CreateSystemManaged<SGActivateEffect>();
            sgActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SActivateEnd>());
            sgActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SAddGrantedAbility>());
            sgActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SAddModifiers>());
            sgActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectAddGrantedTags>());
            sgActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SPlayCueOnActivate>());
            sgActivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SSetEffectActive>());
            sgActivateEffect.SortSystems();

            var sgDeactivateEffect = ExWorld.CreateSystemManaged<SGDeactivateEffect>();
            sgDeactivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SDeactivateEnd>());
            sgDeactivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SRemoveGrantedAbility>());
            sgDeactivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SRemoveModifiers>());
            sgDeactivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectRemoveGrantedTags>());
            sgDeactivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SPlayCueOnDeactivate>());
            sgDeactivateEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SSetEffectDeactive>());
            sgDeactivateEffect.SortSystems();

            var sgRemoveEffect = ExWorld.CreateSystemManaged<SGRemoveEffect>();
            sgRemoveEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectRemoveEnd>());
            sgRemoveEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SPlayCueOnRemove>());
            sgRemoveEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SRemoveEffectFromAscBuffList>());
            sgRemoveEffect.SortSystems();

            sgEffectOperation.AddSystemToUpdateList(sgCheckApplyEffect);
            sgEffectOperation.AddSystemToUpdateList(sgApplyEffect);
            sgEffectOperation.AddSystemToUpdateList(sgCheckActivateEffect);
            sgEffectOperation.AddSystemToUpdateList(sgActivateEffect);
            sgEffectOperation.AddSystemToUpdateList(sgDeactivateEffect);
            sgEffectOperation.AddSystemToUpdateList(sgRemoveEffect);
            sgEffectOperation.SortSystems();

            // Destroy: DestroyEffect
            sgEffectDestroy.AddSystemToUpdateList(ExWorld.CreateSystem<SDestroyEffects>());
            sgEffectDestroy.SortSystems();

            // Tick: RunningEffect
            var sgRunningEffect = ExWorld.CreateSystemManaged<SGRunningEffect>();
            sgRunningEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectDurationTick>());
            sgRunningEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectPeriodTick>());
            sgRunningEffect.AddSystemToUpdateList(ExWorld.CreateSystem<SEffectStackingTick>());
            sgRunningEffect.SortSystems();
            sgEffectTick.AddSystemToUpdateList(sgRunningEffect);
            sgEffectTick.SortSystems();

            // 三级：
            // Apply：InstantEffect,DurationalEffect
            var sgInstantEffectApply = ExWorld.CreateSystemManaged<SGInstantEffect>();
            sgInstantEffectApply.AddSystemToUpdateList(ExWorld.CreateSystem<SExecuteInstantEffectModifiers>());
            sgInstantEffectApply.AddSystemToUpdateList(ExWorld.CreateSystem<SExecuteInstantEffectEnd>());
            sgInstantEffectApply.SortSystems();

            var sgDurationalEffectApply = ExWorld.CreateSystemManaged<SGDurationalEffect>();
            sgDurationalEffectApply.AddSystemToUpdateList(ExWorld.CreateSystem<SAddEffectToAscBuffList>());
            sgDurationalEffectApply.AddSystemToUpdateList(ExWorld.CreateSystem<SCheckEffectStacking>());
            sgDurationalEffectApply.AddSystemToUpdateList(ExWorld.CreateSystem<SPlayCueOnAdd>());
            sgDurationalEffectApply.SortSystems();

            sgApplyEffect.AddSystemToUpdateList(sgInstantEffectApply);
            sgApplyEffect.AddSystemToUpdateList(sgDurationalEffectApply);
            sgApplyEffect.SortSystems();

            #endregion


            /////////////////////////// 表现 系统组 //////////////////////////////

            #region Cue

            var sgDisplay = ExWorld.CreateSystemManaged<SysGrpDisplay>();
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueStart>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueTick>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueEnd>());
            sgDisplay.AddSystemToUpdateList(ExWorld.CreateSystem<SCueDestroy>());
            sgDisplay.SortSystems();

            sgSimulation.AddSystemToUpdateList(sgDisplay);
            sgSimulation.SortSystems();

            #endregion

            // 将world更新同步PlayerLoop
            ScriptBehaviourUpdateOrder.AppendWorldToCurrentPlayerLoop(ExWorld);
        }
    }
}