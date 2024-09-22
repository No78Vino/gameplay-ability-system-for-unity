using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet;
using GAS.RuntimeWithECS.AttributeSet.Component;
using Unity.Entities;

namespace GAS.ECS_TEST_RUNTIME_GEN_LIB
{
    public static class AttrSetContainerExtension
    {
        public static void AddAttrSet(this AttrSetContainer self,int attrSetCode)
        {
            // 如果该属性集已经添加过，则直接返回
            if (!self.TryAddAttrSetCode(attrSetCode)) return;
            
            if (attrSetCode == EcsGAttrSetCode.Fight)
            {
                self.EntityManager.AddComponentData(self.Entity, new ASCom_FIGHT());
            }
            if (attrSetCode == EcsGAttrSetCode.Fight_Monster)
            {
                self.EntityManager.AddComponentData(self.Entity, new ASCom_FIGHT_MONSTER());
            }
        }
        
        // public static AttributeComponent GetAttr(this AttrSetContainer self,int attrSetCode,int attrCode)
        // {
        //     return self.GetAttrSet(attrSetCode).GetAttr(attrCode);
        // }
    }
}