using System;
using BehaviorDesigner.Runtime;
using Demo.Script.UI;
using EXMaidForUI.Runtime.EXMaid;
using GAS.Runtime.Ability;
using GAS.Runtime.Attribute;
using GAS.Runtime.AttributeSet;
using GAS.Runtime.Tags;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Demo.Script.Element
{
    public class BossBladeFang:FightUnit
    {
        public const int HpMax = 300;
        public const int PostureMax = 100;
        public const int ATK = 20;
        public const int Speed = 6;

        protected override string MoveName => AbilityCollection.Move_Info.Name;
        protected override string JumpName => AbilityCollection.Jump_Info.Name;
        protected override string AttackName => AbilityCollection.BossAttack01_Info.Name;
        protected override string DefendName => AbilityCollection.Defend_Info.Name;
        protected override string DodgeName => AbilityCollection.DodgeStep_Info.Name;
        protected override string DieName => AbilityCollection.BossDie_Info.Name;

        [SerializeField] private Player player;
        [SerializeField] private BossCore core;
        
        public BossCore Core => core;


        private bool _dead;
        private bool _outOfPosture;
        private bool _inPhase1;
        public bool ChangingPhase { get; private set; }
        
        public BehaviorTree BT => _bt;
        
        protected override void Awake()
        {
            base.Awake();
            InitAttribute();
            if (player == null) player = FindObjectOfType<Player>();
            
            target = player;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            // 更新dead, outOfPosture, inPhase1 三个状态值
            if (_dead != ASC.AttrSet<AS_Fight>().HP.CurrentValue <= 0)
            {
                _dead = !_dead;
                _bt.SetVariableValue("dead", ASC.AttrSet<AS_Fight>().HP.CurrentValue <= 0);
                _bt.DisableBehavior();
                _bt.EnableBehavior();
            }
            
            if( _outOfPosture != ASC.HasTag(GameplayTagSumCollection.State_Debuff_LoseBalance))
            {
                _outOfPosture = !_outOfPosture;
                _bt.SetVariableValue("outOfPosture", ASC.HasTag(GameplayTagSumCollection.State_Debuff_LoseBalance));
                _bt.DisableBehavior();
                _bt.EnableBehavior();
            }
            
            if (_inPhase1 != ASC.AttrSet<AS_Fight>().HP.CurrentValue > HpMax / 2)
            {
                _inPhase1 = !_inPhase1;
                ChangingPhase = true;
                _bt.SetVariableValue("InPhase1", ASC.AttrSet<AS_Fight>().HP.CurrentValue > HpMax / 2);
                _bt.DisableBehavior();
                _bt.EnableBehavior();

                core.rotateSpeed = 540;
            }
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

        public bool FallDownAttack()
        {
            return ASC.TryActivateAbility(AbilityCollection.BossAttack02_Info.Name);
        }
        
        public bool BeamAttack()
        {
            return ASC.TryActivateAbility(AbilityCollection.BossAttack03_Info.Name);
        }
        
        public bool RoarAttack()
        {
            return ASC.TryActivateAbility(AbilityCollection.BossAttack04_Info.Name);
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

        public void ChangingPhaseEnd()
        {
            ChangingPhase = false;
        }
    }
}