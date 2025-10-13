using Unity.Entities;

namespace GAS.Runtime
{
    public class BasicDataController
    {
        private readonly Entity _asc;
        private CAscBasicData CAscBasicData => GasEntityManager.GetComponentData<CAscBasicData>(_asc);
        
        public BasicDataController(Entity asc)
        {
            _asc = asc;
            GasEntityManager.AddComponentData(_asc, new CAscBasicData());
        }

        private static EntityManager GasEntityManager => GASManager.EntityManager;

        public void SetLevel(int level)
        {
            var bdc = CAscBasicData;
            bdc.Level = level;
            GasEntityManager.SetComponentData(_asc, bdc);
        }

        public int GetLevel() => CAscBasicData.Level;
    }
}