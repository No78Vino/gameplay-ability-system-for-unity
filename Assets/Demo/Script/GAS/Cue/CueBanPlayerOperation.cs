using GAS.Runtime;
using UnityEngine;

namespace GAS.Cue
{
    public class CueBanPlayerOperation:GameplayCueDurational
    {
        public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
        {
            return new CueBanPlayerOperationSpec(this, parameters);
        }
    }

    public class CueBanPlayerOperationSpec : GameplayCueDurationalSpec<CueBanPlayerOperation>
    {
        private readonly Player _player;
        public CueBanPlayerOperationSpec(CueBanPlayerOperation cue, GameplayCueParameters parameters) : base(cue, parameters)
        {
            _player = Owner.GetComponent<Player>();
        }

        public override void OnAdd()
        {
            if(_player) _player.DisableInput();
        }

        public override void OnRemove()
        {
            if(_player) _player.EnableInput();
        }

        public override void OnGameplayEffectActivate()
        {
        }

        public override void OnGameplayEffectDeactivate()
        {
        }

        public override void OnTick()
        {
        }
    }
}