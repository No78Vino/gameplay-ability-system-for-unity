using System;
using System.Collections.Generic;

namespace GAS.Runtime
{
    public static class GasCache
    {
        public static void CacheAttributeSetName(IReadOnlyDictionary<Type, string> attrSetTypeToName)
        {
            AttributeSetUtil.Cache(attrSetTypeToName);
        }
    }
}