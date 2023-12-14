namespace GAS.Runtime.Effects
{
    public class GameplayEffectPeriodTicker
    {
        GameplayEffectSpec _spec;
        private float period => _spec.GameplayEffect.Period;
        float periodRemaining;

        public void Tick()
        {

        }
    }
}