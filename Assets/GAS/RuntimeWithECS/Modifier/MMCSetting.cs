﻿using System;
using Unity.Collections;
using Unity.Entities;

namespace GAS.RuntimeWithECS.Modifier
{
    public struct MMCSetting : IComponentData
    {
        public Type Type;
        public NativeArray<float> floatParams;
        public NativeArray<int> intParams;
        public NativeArray<FixedString32Bytes> stringParams;
    }
}