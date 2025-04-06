using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Helper
{
    public static class ExGasHelper
    {
        public static string GetEntityName(Entity entity)
        {
            string name = GASManager.EntityManager.GetName(entity);
            if (string.IsNullOrEmpty(name)) name =  entity.ToString();
            return name;
        }
    }
}