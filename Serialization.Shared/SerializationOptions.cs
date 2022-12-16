//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Options for serialization and deserialization.
    /// </summary>
    [Flags()]
    public enum SerializationOptions
    {
        /// <summary>
        /// A value indicating that the pointer to the serialized object is never <see langword="null"/>.
        /// </summary>
        PointerNeverNull = 0x00000010,

        /// <summary>
        /// A value indicating that the elements in the serialized object are never <see langword="null"/>.
        /// </summary>
        ElementsNeverNull = 0x00000020,

        /// <summary>
        /// A value indicating that the serialized object can only be an instance of the specified class, and not an instance of a derived class.
        /// </summary>
        FixedType = 0x00000100,
    }
}
