using Demo.Script.Element;
using GAS.Runtime;

namespace Demo.Script.GAS.AbilityTask
{
    public class BossCoreDie:OngoingAbilityTask
    {
        private float speedBase = 1000;
        private float speedFinal = 0;
        private float speedOrigin;

        private BossBladeFang _boss;
        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
            _boss = _spec.Owner.GetComponent<FightUnit>() as BossBladeFang;
        }

        public override void OnStart(int startFrame)
        {
            speedOrigin = _boss.Core.rotateSpeed;
            _boss.Core.rotateSpeed = speedBase;
        }

        public override void OnEnd(int endFrame)
        {
            _boss.Core.rotateSpeed = speedFinal;
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
            // _boss.Core.rotateSpeed线性增加
            float t = (float)(frameIndex - startFrame) / (endFrame - startFrame);
            _boss.Core.rotateSpeed = speedBase + (speedFinal - speedBase) * t;
        }
    }
}