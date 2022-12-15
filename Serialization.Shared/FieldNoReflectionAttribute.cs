//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace System.Runtime.Serialization
{
    /// <summary>
    /// Indicates that this field doesn't allow reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    sealed public class FieldNoReflectionAttribute : Attribute
    {
    }
}
