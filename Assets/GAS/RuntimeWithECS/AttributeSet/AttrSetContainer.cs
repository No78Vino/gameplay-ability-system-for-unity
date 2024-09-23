using System.Collections.Generic;
using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet.Component;
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
    
        public AttrSetContainerComponent Component  => EntityManager.GetComponentData<AttrSetContainerComponent>(Entity);
        
        // 属性集code缓存，便于快速使用
        private List<int> _attrSetCodeList = new();
        private Dictionary<int,int> _attrSetCodeIndexMap = new();
        
        private AttributeComponent GetAttributeComponent(int attrSetCode,int attrCode)
        {
            // 不存在，直接返回null
            if (!_attrSetCodeList.Contains(attrSetCode)) return AttributeComponent.NULL;
            var attrSetIndex = _attrSetCodeIndexMap[attrSetCode];

            var attrSetCom = Component.attributeSets[attrSetIndex];
            var attrIndex = -1;
            for(var i=0;i<attrSetCom.attributeCodes.Length;i++)
            {
                if (attrSetCom.attributeCodes[i] == attrCode)
                {
                    attrIndex = i;
                    break;
                }
            }
            return attrIndex >= 0 ? attrSetCom.attributes[attrIndex] : AttributeComponent.NULL;
        }

        public bool AddAttrSetCode(int attrSetCode)
        {
            if (_attrSetCodeList.Contains(attrSetCode)) return false;
            _attrSetCodeList.Add(attrSetCode);
            _attrSetCodeIndexMap.Add(attrSetCode,_attrSetCodeList.Count-1);
            // TODO:添加属性集数据

            return true;
        }
        
        public float GetBaseValue(int attrSetCode, int attrCode)
        {
            var com = GetAttributeComponent(attrSetCode, attrCode);
            return com.BaseValue;
        }
        
        public float GetCurrentValue(int attrSetCode, int attrCode)
        {
            var com = GetAttributeComponent(attrSetCode, attrCode);
            return com.CurrentValue;
        }
    }
}