using Cysharp.Threading.Tasks;
using GAS.Runtime.Cue;
using TMPro;
using UnityEngine;

namespace GAS.Cue
{
    public enum TipShowType
    {
        Up,
        Shake,
        Zoom
    }
    
    public class CueFloatTip:GameplayCueInstant
    {
        public GameObject prefab;
        public string text;
        public TipShowType showType;
        
        public override GameplayCueInstantSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueFloatTipSpec(this, parameters);
        }
    }

    public class CueFloatTipSpec : GameplayCueInstantSpec<CueFloatTip>
    {
        private float delayTime = 1;
        public CueFloatTipSpec(CueFloatTip cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
        }

        public override void Trigger()
        {
            ShowTip().Forget();
        }

        async UniTask ShowTip()
        {
            var tip = Object.Instantiate(cue.prefab);
            tip.transform.position = Owner.transform.position;
            var text = tip.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
            text.text = cue.text;
            var animator = tip.GetComponent<Animator>();
            animator.Play(cue.showType.ToString());
            
            await UniTask.Delay((int) (delayTime * 1000));
            Object.Destroy(tip);
        }
    }
}