using System;
using Unity.Burst;  
using Unity.Entities;

namespace GAS.Runtime
{
    [UpdateInGroup(typeof(SGActivateEffect))]
    [UpdateBefore(typeof(SActivateEnd))]
    public partial struct SAddGrantedAbility : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WipActivateEffect>();
            state.RequireForUpdate<CEffectInstance>();
            state.RequireForUpdate<MCGrantedAbility>();
            state.RequireForUpdate<CEffectInUsage>();
        }

        // 不能BurstCompile，因为涉及托管组件操作  
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (_, _, grantedAbilityComp, inUsage, ge) in
                     SystemAPI.Query<
                         RefRO<CEffectInstance>,
                         RefRO<WipActivateEffect>,
                         MCGrantedAbility,
                         RefRO<CEffectInUsage>>().WithEntityAccess())
            {
                var targetAsc = inUsage.ValueRO.Target;
                var grantedAbilities = grantedAbilityComp.GrantedAbilities;
                if (grantedAbilities == null || grantedAbilities.Length == 0) continue;

                // 检查是否已有运行时数据（GE重新激活的情况）  
                bool isReactivation = state.EntityManager.HasComponent<MCGrantedAbilityRuntime>(ge);

                if (!isReactivation)
                {
                    // === 首次激活：创建Ability Entity并挂载到ASC ===  
                    var abilityEntities = new Entity[grantedAbilities.Length];
                    var abilityBuffer = SystemAPI.GetBuffer<BAbility>(targetAsc);

                    for (int i = 0; i < grantedAbilities.Length; i++)
                    {
                        var ga = grantedAbilities[i];

                        // 创建Ability Entity（复用AbilityHelper）  
                        var abilityEntity = AbilityHelper.CreateAbilityEntity(ga.AbilityConfig.ComponentConfigs);

                        // 设置Owner为目标ASC  
                        var baseInfo = state.EntityManager.GetComponentData<CAbilityBaseInfo>(abilityEntity);
                        baseInfo.Owner = targetAsc;
                        state.EntityManager.SetComponentData(abilityEntity, baseInfo);

                        // 挂载到ASC的BAbility Buffer  
                        abilityBuffer.Add(new BAbility { Ability = abilityEntity });

                        abilityEntities[i] = abilityEntity;

                        // 根据ActivationPolicy决定是否激活  
                        if (ga.ActivationPolicy == GrantedAbilityActivationPolicy.WhenAdded
                            || ga.ActivationPolicy == GrantedAbilityActivationPolicy.SyncWithEffect)
                        {
                            state.EntityManager.AddComponent<CAbilityInTryActivate>(abilityEntity);
                        }

                        // 注册自移除回调（WhenEnd / WhenCancel / WhenCancelOrEnd）  
                        RegisterSelfRemovalCallback(ga.RemovePolicy, abilityEntity, targetAsc);
                    }

                    // 存储运行时引用到GE Entity  
                    state.EntityManager.AddComponent<MCGrantedAbilityRuntime>(ge);
                    state.EntityManager.SetComponentData(ge, new MCGrantedAbilityRuntime(abilityEntities));
                }
                else
                {
                    // === 重新激活：只处理SyncWithEffect策略的激活 ===  
                    var runtime = state.EntityManager.GetComponentData<MCGrantedAbilityRuntime>(ge);
                    if (runtime.GrantedAbilityEntities == null) continue;

                    for (int i = 0; i < grantedAbilities.Length; i++)
                    {
                        if (grantedAbilities[i].ActivationPolicy != GrantedAbilityActivationPolicy.SyncWithEffect)
                            continue;

                        var abilityEntity = runtime.GrantedAbilityEntities[i];
                        if (abilityEntity == Entity.Null) continue;
                        if (state.EntityManager.HasComponent<CAbilityActive>(abilityEntity)) continue;

                        state.EntityManager.AddComponent<CAbilityInTryActivate>(abilityEntity);
                    }
                }
            }
        }

        private static void RegisterSelfRemovalCallback(
            GrantedAbilityRemovePolicy removePolicy,
            Entity abilityEntity,
            Entity targetAsc)
        {
            // 用局部变量持有Action引用，以便回调内注销自身  
            Action onEnd = null;
            Action onCancel = null;

            switch (removePolicy)
            {
                case GrantedAbilityRemovePolicy.WhenEnd:
                    onEnd = () =>
                    {
                        RemoveAbilityFromAsc(abilityEntity, targetAsc);
                        GASEventCenter.UnRegisterOnEndAbility(abilityEntity, onEnd);
                    };
                    GASEventCenter.RegisterOnEndAbility(abilityEntity, onEnd);
                    break;

                case GrantedAbilityRemovePolicy.WhenCancel:
                    onCancel = () =>
                    {
                        RemoveAbilityFromAsc(abilityEntity, targetAsc);
                        GASEventCenter.UnRegisterOnCancelAbility(abilityEntity, onCancel);
                    };
                    GASEventCenter.RegisterOnCancelAbility(abilityEntity, onCancel);
                    break;

                case GrantedAbilityRemovePolicy.WhenCancelOrEnd:
                    onEnd = () =>
                    {
                        RemoveAbilityFromAsc(abilityEntity, targetAsc);
                        GASEventCenter.UnRegisterOnEndAbility(abilityEntity, onEnd);
                        if (onCancel != null) GASEventCenter.UnRegisterOnCancelAbility(abilityEntity, onCancel);
                    };
                    onCancel = () =>
                    {
                        RemoveAbilityFromAsc(abilityEntity, targetAsc);
                        GASEventCenter.UnRegisterOnCancelAbility(abilityEntity, onCancel);
                        if (onEnd != null) GASEventCenter.UnRegisterOnEndAbility(abilityEntity, onEnd);
                    };
                    GASEventCenter.RegisterOnEndAbility(abilityEntity, onEnd);
                    GASEventCenter.RegisterOnCancelAbility(abilityEntity, onCancel);
                    break;
            }
        }

        /// <summary>  
        /// 从ASC的BAbility Buffer中移除指定Ability  
        /// </summary>  
        private static void RemoveAbilityFromAsc(Entity abilityEntity, Entity targetAsc)
        {
            if (!GASManager.EntityManager.Exists(targetAsc)) return;
            var buffer = GASManager.EntityManager.GetBuffer<BAbility>(targetAsc);
            for (int j = 0; j < buffer.Length; j++)
            {
                if (buffer[j].Ability == abilityEntity)
                {
                    buffer.RemoveAt(j);
                    break;
                }
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }
}