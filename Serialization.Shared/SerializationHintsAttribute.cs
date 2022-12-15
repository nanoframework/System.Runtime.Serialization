//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Provides hints to the binary serializer on how to improve serialization and decrease the size of the serialialized representation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field
                    | AttributeTargets.Property
                    | AttributeTargets.Class, Inherited = true)]
    public class SerializationHintsAttribute : Attribute
    {

#pragma warning disable S1104 // intended use in .NET nanoFramework for native usage

        /// <summary>
        /// Serialization options for the current object.
        /// </summary>
        public SerializationOptions Options;

        /// <summary>
        /// Specifies the size of an array.
        /// </summary>
        /// <remarks>
        /// If an array's size is fixed and known, it can be stated using the <see cref="ArraySize"/> option. A value of -1 can be used if the class being serialized only has one array of simple data types and no other fields.
        /// </remarks>
        //// -1 == extend to the end of the stream.
        public int ArraySize;

        /// <summary>
        /// Specifies the number of bits in which the current object is bit-packed.
        /// </summary>
        /// <remarks>
        /// This can be applied only to ordinal types, <see cref="DateTime"/> and <see cref="TimeSpan"/>.
        /// This indicates how many bits should be kept for an ordinal type. If <see cref="BitPacked"/> is 0, a data type's default size is applied (for example, 2 bytes for an <see cref="short"/> and 1 byte for the <see cref="byte"/> type). 
        /// Decorating a <see cref="bool"/> field with <see cref="BitPacked"/> attribute set to 1, only one bit - which is enough to represent true or false - is kept for the <see cref="bool"/> value.
        /// </remarks>
        //// bits count
        public int BitPacked;

        /// <summary>
        /// Specifies the range bias adjustment for a particular serialized value.
        /// </summary>
        /// <remarks>
        /// This can be applied to ordinal types, <see cref="DateTime"/> and <see cref="TimeSpan"/>.
        /// It can't be applied to <see cref="bool"/> type objects.
        /// With the exception of <see cref="bool"/>, all ordinal types can use <see cref="RangeBias"/>. It allows to store a value using fewer bits. Before saving the ordinal value, the range bias value is subtracted from it. For instance, a 16 bit data type has to used to store values that range between 1000 and 1500. The required bits can be decreased from 16 to 6 if the range is known in advance. When <see cref="RangeBias"/> is set to 1000, the number 1000 is subtracted from the original value before it's serialized.
        /// </remarks>
        public long RangeBias;

        /// <summary>
        /// Specifies the range bias adjustment for a particular serialized value.
        /// </summary>
        /// <remarks>
        /// This can be applied to ordinal types, <see cref="DateTime"/> and <see cref="TimeSpan"/>.
        /// It can't be applied to <see cref="bool"/> type objects.
        /// When serializing a value, <see cref="Scale"/> helps save storage bits similar to <see cref="RangeBias"/>. The value to be serialized is divided by <see cref="Scale"/>. Defining a <see cref="Scale"/> of two, the range of values can be cut in half if a field only contains odd or even values. It's also possible to combine <see cref="RangeBias"/> with <see cref="Scale"/>. However, the order of operations must be taken into account here. The value will be first subtracted by <see cref="RangeBias"/> and then divided by <see cref="Scale"/>.
        /// </remarks>
        //// this will be ticks for DateTime objects
        public ulong Scale;

#pragma warning restore S1104 // Fields should not have public accessibility

    }
}
