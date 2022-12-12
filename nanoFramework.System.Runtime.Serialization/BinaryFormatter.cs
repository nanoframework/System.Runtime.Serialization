//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Runtime.CompilerServices;

namespace System.Runtime.Serialization.Formatters.Binary
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryFormatter"/> class.
    /// </summary>
    public static class BinaryFormatter
    {
        /// <summary>
        /// Serializes the object, or graph of objects with the specified top (root), to a byte array.
        /// </summary>
        /// <param name="graph">The object at the root of the graph to serialize.</param>
        /// <returns>A byte array with the serialized graph.</returns>
        /// <remarks>This implementation is specific of .NET nanoFramework.</remarks>
        /// <exception cref="SerializationException">An error has occurred during serialization.</exception>
        [MethodImpl(MethodImplOptions.InternalCall)]
#pragma warning disable S4200 // Intended usage in .NET nanoFramework
        extern static public byte[] Serialize(object graph);
#pragma warning restore S4200 // Native methods should be wrapped

        /// <summary>
        /// Deserializes the specified byte array into an object graph.
        /// </summary>
        /// <param name="buffer">The byte array from which to deserialize the object graph.</param>
        /// <returns>The top (root) of the object graph.</returns>
        /// <remarks>This implementation is specific of .NET nanoFramework.</remarks>
        /// <exception cref="SerializationException">An error occurred while deserializing an object from the byte array.</exception>
        [MethodImpl(MethodImplOptions.InternalCall)]
#pragma warning disable S4200 // Intended usage in .NET nanoFramework
        extern static public object Deserialize(byte[] buffer);
#pragma warning restore S4200 // Native methods should be wrapped
    }
}
