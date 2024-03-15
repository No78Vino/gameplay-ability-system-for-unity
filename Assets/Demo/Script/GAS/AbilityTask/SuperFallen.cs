using Cysharp.Threading.Tasks;
using GAS.Runtime;
using UnityEngine;

namespace Demo.Script.GAS.AbilityTask
{
    public class SuperFallen : OngoingAbilityTask
    {
        private FightUnit _unit;
        private Vector2 _groundPoint;
        private Vector2 _start;
        private readonly LayerMask _groundLayer = LayerMask.GetMask("Terrain");
        public override void Init(AbilitySpec spec)
        {
            base.Init(spec);
            _unit = _spec.Owner.GetComponent<FightUnit>();
        }

        public override void OnStart(int startFrame)
        {
            _start= _spec.Owner.transform.position;
            // 发射射线
            var hit = Physics2D.Raycast(_start, Vector2.down, Mathf.Infinity, _groundLayer);

            if (hit.collider != null)
            {
                _groundPoint = hit.point;
            }
            else
            {
                _groundPoint = _start + Vector2.down * 100;
            }
        }

        public override void OnEnd(int endFrame)
        {
            _unit.Rb.MovePosition(_groundPoint);
        }

        public override void OnTick(int frameIndex, int startFrame, int endFrame)
        {
            float t = (float)(frameIndex - startFrame) / (endFrame - startFrame);
            _unit.Rb.MovePosition(Vector3.Lerp(_start, _groundPoint, t));
        }
    }
}