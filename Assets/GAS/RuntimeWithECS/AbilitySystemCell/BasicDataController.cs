using GAS.RuntimeWithECS.AbilitySystemCell.Component;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AbilitySystemCell
{
    public class BasicDataController
    {
        private readonly Entity _asc;
        private BasicDataComponent BasicData => GasEntityManager.GetComponentData<BasicDataComponent>(_asc);
        
        public BasicDataController(Entity asc)
        {
            _asc = asc;
            GasEntityManager.AddComponentData(_asc, new BasicDataComponent());
        }

        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public void SetLevel(int level)
        {
            var bdc = BasicData;
            bdc.Level = level;
            GasEntityManager.SetComponentData(_asc, bdc);
        }

        public int GetLevel() => BasicData.Level;
    }
}