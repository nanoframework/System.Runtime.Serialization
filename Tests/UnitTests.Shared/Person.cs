//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Runtime.Serialization;

namespace nanoFramework.System.Runtime.Serialization.Tests
{
    [Serializable]
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address;
        public DateTime Birthday { get; set; }
        public int ID { get; set; }
        [SerializationHints(ArraySize = 2)]
        public string[] ArrayProperty { get; set; }
        public Person Friend { get; set; }
    }
}
