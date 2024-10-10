using Unity.Entities;

namespace GAS.RuntimeWithECS.Ability
{
    public class GameplayAbilityController
    {
        private readonly Entity _asc;
        
        public GameplayAbilityController(Entity asc)
        {
            _asc = asc;
        }
    }
}