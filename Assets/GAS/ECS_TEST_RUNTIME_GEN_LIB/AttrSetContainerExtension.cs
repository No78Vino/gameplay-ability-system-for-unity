using GAS.RuntimeWithECS.Attribute.Component;
using GAS.RuntimeWithECS.AttributeSet;
using GAS.RuntimeWithECS.AttributeSet.Component;
using Unity.Entities;

namespace GAS.ECS_TEST_RUNTIME_GEN_LIB
{
    public static class AttrSetContainerExtension
    {
        // public static void AddAttrSet(this AttrSetContainer self,int attrSetCode)
        // {
        //     switch (attrSetCode)
        //     {
        //         case EcsGAttrSetCode.Fight:
        //         {
        //             var com = new ASCom_FIGHT();
        //             if (self.EntityManager.AddComponentData(self.Entity, com))
        //                 self.TryAddAttrSetCode(attrSetCode, com);
        //             break;
        //         }
        //         case EcsGAttrSetCode.Fight_Monster:
        //         {
        //             var com = new ASCom_FIGHT();
        //             if (self.EntityManager.AddComponentData(self.Entity, com))
        //                 self.TryAddAttrSetCode(attrSetCode, com);
        //             break;
        //         }
        //     }
        // }
        
        // public static AttributeComponent GetAttribute(this AttrSetContainer self,int attrSetCode,int attrCode)
        // {
        //     // 不存在，直接返回null
        //     if (!self.AttrSetCodeMap.TryGetValue(attrSetCode, out var attrSetData)) return AttributeComponent.NULL;
        //     if (attrSetCode == EcsGAttrSetCode.Fight)
        //     {
        //         if(attrCode==EcsGAttrLib.HP)
        //             return ((ASCom_FIGHT)attrSetData).HP;
        //     }
        //     return AttributeComponent.NULL;
        // }
    }
}