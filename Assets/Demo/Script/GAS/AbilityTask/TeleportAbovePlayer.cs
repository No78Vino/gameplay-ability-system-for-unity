using System;
using Demo.Script.Element;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using GAS.Editor;
#endif
using GAS.Runtime;
using UnityEngine;

namespace Demo.Script.GAS.AbilityTask
{
    [Serializable]
    public class TeleportAbovePlayer:InstantAbilityTask
    {
        public float TeleportHeight = 6;
        
        public override void OnExecute()
        {
            var boss = _spec.Owner.GetComponent<FightUnit>() as BossBladeFang;
            boss.transform.position = boss.target.transform.position + new UnityEngine.Vector3(0, TeleportHeight, 0);
        }
    }
    
    #if UNITY_EDITOR
    public class TeleportAbovePlayerInspector : InstantTaskInspector<TeleportAbovePlayer>
    {
        [Delayed] [LabelText("瞬移高度")] [OnValueChanged("OnHeightChanged")]
        public float TeleportHeight;
        
        public override void Init(InstantAbilityTask task)
        {
            base.Init(task);
            TeleportHeight = _task.TeleportHeight;
        }

        void OnHeightChanged()
        {
            _task.TeleportHeight = TeleportHeight;
            Save();
        }
    }
#endif
}