//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Runtime.Serialization
{
    /// <summary>
    /// The exception thrown when an error occurs during serialization or deserialization.
    /// </summary>
    public class SerializationException : Exception
    {
        /// <summary>
        /// The type for which deserialization has failed.
        /// </summary>
#pragma warning disable S1104 // Intend usage. Meant to be used at native end.
        public Type Type;
#pragma warning restore S1104 // Fields should not have public accessibility
    }
}
