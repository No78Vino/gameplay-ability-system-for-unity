using Unity.Entities;

namespace GAS.Runtime
{
    public static class ExGasHelper
    {
        public static string GetEntityName(Entity entity)
        {
            if (!GASManager.IsInitialized) return string.Empty;
  
            string name = GASManager.EntityManager.GetName(entity);
            if (string.IsNullOrEmpty(name)) name =  entity.ToString();
            return name;
        }
    }
}