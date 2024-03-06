using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using UnityEngine;

namespace Demo.Script.Element
{
    public class BossBladeFang:FightUnit
    {
        public const int HpMax = 300;
        public const int PostureMax = 100;
        public const int ATK = 20;
        public const int Speed = 12;

        protected override string MoveName => AbilityCollection.Move_Info.Name;
        protected override string JumpName => AbilityCollection.Jump_Info.Name;
        protected override string AttackName => AbilityCollection.Attack_Info.Name;
        protected override string DefendName => AbilityCollection.Defend_Info.Name;
        protected override string DodgeName => AbilityCollection.DodgeStep_Info.Name;

        [SerializeField] private Player player;
        
        protected override void Awake()
        {
            base.Awake();
            InitAttribute();
            target = player;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ASC.AttrSet<AS_Fight>().POSTURE.RegisterPreBaseValueChange(OnPostureChangePre);
            ASC.AttrSet<AS_Fight>().POSTURE.RegisterPostBaseValueChange(OnPostureChangePost);

            ASC.AttrSet<AS_Fight>().HP.RegisterPreBaseValueChange(OnHpChangePre);
            ASC.AttrSet<AS_Fight>().HP.RegisterPostBaseValueChange(OnHpChangePost);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ASC.AttrSet<AS_Fight>().POSTURE.UnregisterPostBaseValueChange(OnPostureChangePost);
            ASC.AttrSet<AS_Fight>().POSTURE.UnregisterPreBaseValueChange(OnPostureChangePre);

            ASC.AttrSet<AS_Fight>().HP.UnregisterPreBaseValueChange(OnHpChangePre);
            ASC.AttrSet<AS_Fight>().HP.UnregisterPostBaseValueChange(OnHpChangePost);
        }
        
        public override void InitAttribute()
        {
            ASC.AttrSet<AS_Fight>().InitHP(HpMax);
            ASC.AttrSet<AS_Fight>().InitPOSTURE(0);
            ASC.AttrSet<AS_Fight>().InitATK(ATK);
            ASC.AttrSet<AS_Fight>().InitSPEED(Speed);
        }
        
        private float OnPostureChangePre(AttributeBase attr, float newValue)
        {
            return Mathf.Clamp(newValue, 0, PostureMax);
        }

        private void OnPostureChangePost(AttributeBase attr, float oldValue, float newValue)
        {
            Debug.Log($"Boss Posture changed from {oldValue} to {newValue}");
            XUI.M.VM<MainUIVM>().UpdateBossPosture(newValue);
        }

        private float OnHpChangePre(AttributeBase attr, float newValue)
        {
            return Mathf.Clamp(newValue, 0, HpMax);
        }

        private void OnHpChangePost(AttributeBase attr, float oldValue, float newValue)
        {
            Debug.Log($"Boss HP changed from {oldValue} to {newValue}");
            XUI.M.VM<MainUIVM>().UpdateBossHp(newValue);
        }

        public override bool CatchTarget()
        {
            if (target == null)
                return false;

            var deltaVector3 = target.transform.position - transform.position;
            float distance = deltaVector3.magnitude;
            return distance < 6;
        }
    }
}