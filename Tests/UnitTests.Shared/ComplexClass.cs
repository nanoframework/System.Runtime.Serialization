//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;

namespace nanoFramework.System.Runtime.Serialization.Tests
{
    [Serializable]
    public class ComplexClass
    {
        public int aInteger { get; set; }
        public short aShort { get; set; }
        public byte aByte { get; set; }
        public string aString { get; set; }
        public float aFloat { get; set; }
        public double aDouble { get; set; }
        public bool aBoolean { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime FixedTimestamp { get; set; }
        public int[] intArray { get; set; }
        public short[] shortArray { get; set; }
        public byte[] byteArray { get; set; }
        public string[] stringArray { get; set; }
        public float[] floatArray { get; set; }
        public double[] doubleArray { get; set; }
        public ChildClass child1;
        public ChildClass Child { get; set; }
        public object nullObject { get; set; }
        public float nanFloat { get; set; }
        public double nanDouble { get; set; }
        private readonly string dontSerializeStr = "dontPublish";
        private string dontSerialize { get; set; } = "dontPublish";
    }

    [Serializable]
    public class ChildClass
    {
        public int one { get; set; }
        public int two { get; set; }
        public int three { get; set; }
        public int four; //not a property on purpose!
    }
}
