using System;
using Unity.Entities;
using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 面向使用EX-GAS开发者的GameplayCue控制单位，可以理解为Cue面向对象开发的伪装类
    /// GameplayCue设计上允许作为一个独立系统被使用。因此给出了这个类，用于GAS外部使用。
    /// </summary>
    public class GameplayCueUnit
    {
        private Entity _cueEntity;
        private Type _cueType;
        private XParam _xParam;
        private AbilitySystemCell _asc;
        private int[] _requiredAllTags;
        private int[] _requiredAnyTags;
        private int[] _requiredNoneTags;
        private int[] _immunityAllTags;
        private int[] _immunityAnyTags;
        private int[] _immunityNoneTags;
        
        private static EntityManager EntityManager=>GASManager.EntityManager;
        
        public Type CueType => _cueType;
        public XParam Param => _xParam;
        /// <summary>
        /// GameplayCue独立控制单位
        /// </summary>
        /// <param name="cueType">Cue 类型</param>
        /// <param name="xParam">Cue 对应的自定义参数</param>
        /// <param name="requiredTags">可选：添加到ASC时，ASC播放需求的tag</param>
        /// <param name="immunityTags">>可选：添加到ASC时，ASC播放免疫的tag</param>
        public GameplayCueUnit(Type cueType,XParam xParam,int[] requiredTags = null, int[] immunityTags = null)
        {
            _cueType = cueType;
            _xParam = xParam;
            _requiredAllTags = TagHelper.FilterInvalidTags(requiredTags);
            _requiredAnyTags = Array.Empty<int>();
            _requiredNoneTags = Array.Empty<int>();
            _immunityAllTags = Array.Empty<int>();
            _immunityAnyTags = Array.Empty<int>();
            _immunityNoneTags = TagHelper.FilterInvalidTags(immunityTags);
        }
        
        public GameplayCueUnit(GameplayCueConfig config)
        {
            _cueType = config.CueType;
            _xParam = config.Param;
            _requiredAllTags = TagHelper.FilterInvalidTags(config.RequiredAllTags);
            _requiredAnyTags = TagHelper.FilterInvalidTags(config.RequiredAnyTags);
            _requiredNoneTags = TagHelper.FilterInvalidTags(config.RequiredNoneTags);
            _immunityAllTags = TagHelper.FilterInvalidTags(config.ImmunityAllTags);
            _immunityAnyTags = TagHelper.FilterInvalidTags(config.ImmunityAnyTags);
            _immunityNoneTags = TagHelper.FilterInvalidTags(config.ImmunityNoneTags);
        }

        private bool CheckCueEntity()
        {
            if (_cueEntity == Entity.Null || !EntityManager.Exists(_cueEntity))
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] cue运行用实例不存在，请先创建cue. 【 调用Create() 】");
#endif
                return false;
            }

            return true;
        }
        
        private bool EvaluateTagRequirement(Entity asc, int[] all, int[] any, int[] none)
        {
            bool passAll = all == null || all.Length == 0 || ASCHelper.HasAllTags(asc, all);
            bool passAny = any == null || any.Length == 0 || ASCHelper.HasAnyTags(asc, any);
            bool passNone = none == null || none.Length == 0 || !ASCHelper.HasAnyTags(asc, none);
            return passAll && passAny && passNone;
        }

        /// <summary>
        /// 创建GameplayCue运行用的实例
        /// </summary>
        public void Create()
        {
            if (_cueEntity != Entity.Null && EntityManager.Exists(_cueEntity))
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Cue已经创建过了，不能重复创建。");
#endif
                return;
            }
            _cueEntity = EntityManager.CreateEntity();
            EntityHelper.SetName(_cueEntity,$"Cue_{_cueType.Name}_{_cueEntity.Version}_{_cueEntity.Index}");
            
            var mcCue = new MCCue(CueHelper.TryCreateCue(_cueType, _xParam));
            mcCue.cue.SetCueEntity(_cueEntity);
            mcCue.cue.SetSourceEntity(Entity.Null, CueSourceType.None);
            EntityHelper.AddManagedComponent<MCCue>(_cueEntity);
            EntityHelper.SetManagedComponent(_cueEntity,mcCue);
            
            EntityHelper.AddComponent<ECCuePlayable>(_cueEntity);
            EntityHelper.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
            
            EntityHelper.AddComponent<ECCuePlaying>(_cueEntity);
            EntityHelper.SetComponentEnabled<ECCuePlaying>(_cueEntity,false);
            
            EntityHelper.AddComponent<ECKillCue>(_cueEntity);
            EntityHelper.SetComponentEnabled<ECKillCue>(_cueEntity,false);
        }
        
        /// <summary>
        ///  销毁GameplayCue运行用的实例
        /// </summary>
        public void Destroy()
        {
            
            if (!CheckCueEntity())
            {
#if UNITY_EDITOR
                Debug.LogError($"[EX] Cue没有创建过或已被销毁，不能重复销毁。");
#endif
                return;
            }

            EntityHelper.SetComponentEnabled<ECKillCue>(_cueEntity,true);
            var mcCue = EntityHelper.GetManagedComponentData<MCCue>(_cueEntity);
            mcCue.cue.OnRemove(Time.time);
            _cueEntity = Entity.Null;
        }
        
        /// <summary>
        ///    添加GameplayCue到ASC
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public bool AddToAsc(AbilitySystemCell asc)
        {
            return CheckCueEntity() && AddToAsc(asc.Entity);
        }
        
        /// <summary>
        ///   添加GameplayCue到ASC
        /// </summary>
        /// <param name="asc"></param>
        /// <returns></returns>
        public bool AddToAsc(Entity asc)
        {
            if (!CheckCueEntity()) return false;
            
            if (!EvaluateTagRequirement(asc, _requiredAllTags, _requiredAnyTags, _requiredNoneTags)) return false;
            if (!EvaluateTagRequirement(asc, _immunityAllTags, _immunityAnyTags, _immunityNoneTags)) return false;

            var mcCue = EntityHelper.GetManagedComponentData<MCCue>(_cueEntity);
            mcCue.cue.AddToTargetAsc(asc);
            
            _asc = GASManager.GetAscFromEntity(asc);
            return true;
        }
        
        
        /// <summary>
        ///     从ASC移除GameplayCue
        /// </summary>
        public void RemoveFromAsc()
        {
            if (!CheckCueEntity()) return;
            
            var mcCue = EntityHelper.GetManagedComponentData<MCCue>(_cueEntity);
            mcCue.cue.RemoveFromTargetAsc();
        }
        
        /// <summary>
        ///   播放GameplayCue
        /// </summary>
        public void Play()
        {
            if (!CheckCueEntity()) return;
            EntityHelper.SetComponentEnabled<ECCuePlayable>(_cueEntity,true);
        }
        
        /// <summary>
        ///     停止GameplayCue
        /// </summary>
        public void Stop()
        {
            if (!CheckCueEntity()) return;
            EntityHelper.SetComponentEnabled<ECCuePlayable>(_cueEntity,false);
        }
        
        /// <summary>
        ///     手动Tick
        ///     TODO
        /// </summary>
        public void Tick()
        {
        }
        
        /// <summary>
        ///   设置GameplayCue来源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sourceType"></param>
        public void SetSource(Entity source, CueSourceType sourceType)
        {
            if (!CheckCueEntity()) return;
            var mcCue = EntityHelper.GetManagedComponentData<MCCue>(_cueEntity);
            mcCue.cue.SetSourceEntity(source, sourceType);
            EntityHelper.SetManagedComponent(_cueEntity,mcCue);
        }

#if UNITY_EDITOR
        /// <summary>
        ///     编辑器预览Cue效果
        ///     注意：该方法只在编辑器下有效，运行时无效。
        ///     请使用 UNITY_EDITOR 宏来包裹该方法，否则在运行时会导致编译错误。
        /// </summary>
        /// <param name="target"></param>
        /// <param name="frame"></param>
        /// <param name="startFrame"></param>
        /// <param name="endFrame"></param>
        public void OnPreview(GameObject target,int frame, int startFrame, int endFrame)
        {
            var cue = CueHelper.TryCreateCue(_cueType, _xParam);
            cue.OnPreview(target,frame, startFrame, endFrame);
        }
#endif
    }
}
