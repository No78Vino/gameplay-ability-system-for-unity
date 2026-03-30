using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    public static class CueHelper
    {
        public static GameplayCueBase TryCreateCue(GameplayCueConfig param)
        {
            return TryCreateCue(param.CueType, param.Param);
        }
        
        public static GameplayCueBase TryCreateCue(string cueType, XParam param)
        {
            if (CueTypeMap.TryGetValue(cueType, out var type))
                return TryCreateCue(type, param);
#if UNITY_EDITOR
            Debug.LogError($"[EX] 创建Cue失败:Can't find Cue for cueType [{cueType}]. " +
                           "Cue的Type映射脚本错误，请重新生成。");
#endif
            return null;
        }

        public static GameplayCueBase TryCreateCue(Type type, XParam param)
        {
            try
            {
                if (Activator.CreateInstance(type) is GameplayCueBase cue)
                {
                    cue.InitParameters(param);
                    return cue;
                }
            }
            catch (MissingMethodException e)
            {
                Debug.LogError("[EX] 创建Cue失败: " +
                               $"请检查这个类【'{type.FullName}'】是否继承自NewGameplayCueBase;" +
                               "或者，GameplayCueBase的Type映射脚本是否更新，重新生成。" +
                               $"Error Exception:{e.Message}");
                throw;
            }
            return null;
        }

        public static XParam CreateCueParameter(string type, List<object> paramData = null)
        {
            var cueParamConfigType = GetCueLogicParamType(type);
            var cueParamEditor = (XParam)Activator.CreateInstance(cueParamConfigType);
#if UNITY_EDITOR
            if (paramData != null) cueParamEditor.DecodeExcelData(paramData);
#endif
            return cueParamEditor;
        }

        public static Type GetCueLogicParamType(string cueType)
        {
            return CueType2CueParamTypeMap.TryGetValue(cueType,out var cueParam) ? CueParamTypeMap[cueParam] : null;
        }
        
        public static Type GetCueLogicParamType(Type cueType)
        {
            var cueParam = CueType2CueParamTypeMap[cueType.Name];
            return CueParamTypeMap[cueParam];
        }
        
        #region Cue

        private static readonly Dictionary<string, Type> CueTypeMap = new();
        private static readonly Dictionary<string, Type> CueParamTypeMap = new();
        private static readonly Dictionary<string, string> CueType2CueParamTypeMap = new();

        public static void RegisterCue(string sType, Type logicType,Type cueParamType)
        {
            CueTypeMap[sType] = logicType;
            CueParamTypeMap[cueParamType.Name] = cueParamType;
            CueType2CueParamTypeMap[sType] = cueParamType.Name;
        }
        
        public static Type GetCueType(string sType)
        {
            if (CueTypeMap.TryGetValue(sType, out var type)) return type;
#if UNITY_EDITOR
            Debug.LogError($"[EX] CueTypeMap中没有找到类型: {sType}，请检查是否注册了该Cue类型。");
#endif
            return null;
        }

        public static void RegisterCue<T>(string sType,Type cueParam) where T : GameplayCueBase
        {
            RegisterCue(sType, typeof(T),cueParam);
        }

        public static List<string> GetCueTypeNames()
        {
            return CueTypeMap.Keys.ToList();
        }
        
        #endregion

        [BurstCompile]
        public static void StopCue(Entity cueEntity,EntityManager entityManager)
        {
            if (entityManager.IsComponentEnabled<ECCuePlaying>(cueEntity))
                entityManager.SetComponentEnabled<ECCuePlayable>(cueEntity,false);
        }
        
        [BurstCompile]
        public static void PlayCue(Entity cueEntity,EntityManager entityManager)
        {
            if (!entityManager.IsComponentEnabled<ECCuePlaying>(cueEntity))
                entityManager.SetComponentEnabled<ECCuePlayable>(cueEntity,true);
        }

        public static MCCue InitInstantCueFromGameplayEffect(MCCue cue,Entity cueEntity,Entity ge)
        {
            cue.cue.SetSourceEntity(ge,CueSourceType.GameplayEffect);
            cue.cue.SetCueEntity(cueEntity);
            return cue;
        }
        
        public static MCCue CopyCueComponent(MCCue cue)
        {
            return new MCCue()
            {
                cue = cue.cue
            };
        }

        #region 通用型工具接口

        private static bool EvaluateAscTagRequirement(Entity asc, in TagRequirementData requirement)
        {
            bool passAll = !requirement.all.IsCreated || requirement.all.Length == 0 || ASCHelper.HasAllTags(asc, requirement.all);
            bool passAny = !requirement.any.IsCreated || requirement.any.Length == 0 || ASCHelper.HasAnyTags(asc, requirement.any);
            bool passNone = !requirement.none.IsCreated || requirement.none.Length == 0 || !ASCHelper.HasAnyTags(asc, requirement.none);
            return passAll && passAny && passNone;
        }

        public static void TryPlayCueOnAsc(EntityManager entityManager, Entity targetAsc, Entity cueEntity, Entity sourceGE)
        {
            // 1.先判断tag是否可以播放cue
            if (entityManager.HasComponent<CPlayRequiredTags>(cueEntity))
            {
                var requiredTags = entityManager.GetComponentData<CPlayRequiredTags>(cueEntity);
                if (!EvaluateAscTagRequirement(targetAsc, requiredTags.requirement)) return;
            }
            if (entityManager.HasComponent<CPlayImmunitedTags>(cueEntity))
            {
                var immunityTags = entityManager.GetComponentData<CPlayImmunitedTags>(cueEntity);
                if (!EvaluateAscTagRequirement(targetAsc, immunityTags.requirement)) return;
            }
            // 2.重置Cue逻辑单元
            var cueLogic = entityManager.GetComponentData<MCCue>(cueEntity);
            cueLogic.cue.Reset();
            cueLogic.cue.SetSourceEntity(sourceGE, CueSourceType.GameplayEffect);
            cueLogic.cue.AddToTargetAsc(targetAsc);
            // 3.激活CuePlaying
            cueLogic.cue.Play(true);
        }
        #endregion
    }
}
