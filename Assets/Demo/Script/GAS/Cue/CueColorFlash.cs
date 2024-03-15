using Cysharp.Threading.Tasks;
using GAS.Runtime;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GAS.Cue
{
    public class CueColorFlash : GameplayCueInstant
    {
        [BoxGroup] [LabelText("闪变颜色")] [LabelWidth(100)]
        public Color color;

        [BoxGroup] [LabelText("闪变时间")] [LabelWidth(100)]
        public float time;

        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueColorFlashSpec(this, parameters);
        }
    }

    public class CueColorFlashSpec : GameplayCueInstantSpec<CueColorFlash>
    {
        public CueColorFlashSpec(CueColorFlash cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
        }

        public override void Trigger()
        {
            Flash().Forget();
        }

        private async UniTask Flash()
        {
            var spriteRenderers = Owner.transform.Find("Root").GetComponentsInChildren<SpriteRenderer>();
            
            foreach (var sr in spriteRenderers) sr.color = cue.color;
            
            await UniTask.Delay((int)(cue.time * 1000));
            
            foreach (var sr in spriteRenderers) sr.color = Color.white;
        }
    }
}