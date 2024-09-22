using System.Collections.Generic;
using GAS.RuntimeWithECS.Core;
using Unity.Entities;

namespace GAS.RuntimeWithECS.AttributeSet
{
    public class AttrSetContainer
    {
        public AttrSetContainer(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; private set; }

        public EntityManager EntityManager => GASManager.EntityManager;

        public List<int> AttrSetCodeList { get; } = new();

        public bool TryAddAttrSetCode(int attrSetCode)
        {
            if (AttrSetCodeList.Contains(attrSetCode)) return false;
            AttrSetCodeList.Add(attrSetCode);
            return true;
        }
        // 添加属性集组件的函数在拓展类中实现，详见AttrSetContainerExtension
        // public void AddAttrSet(int attrSetCode)
    }
}