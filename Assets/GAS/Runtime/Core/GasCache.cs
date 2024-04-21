using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public class GasCache
    {
        public static void CacheAttributeSetName(Dictionary<Type, string> attrSetTypeToName)
        {
            AttributeSetUtil.Cache(attrSetTypeToName);
        }
    }
}