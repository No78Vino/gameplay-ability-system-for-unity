using Unity.Entities;

namespace GAS.RuntimeWithECS.Tag
{
    public class GameplayTagController
    {
        private Entity _asc;
        public GameplayTagController(Entity asc)
        {
            _asc = asc;
        } 
    }
}