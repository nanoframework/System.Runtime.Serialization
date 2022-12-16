//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

#if NANOFRAMEWORK_1_0
using nanoFramework.TestFramework;
using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nanoFramework.Serialization.Helper;
#endif

namespace nanoFramework.System.Runtime.Serialization.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializePersonClassTest()
        {
            // serialize person class
            var personOne = UnitTestHelper.CreatePersonOne();

#if NANOFRAMEWORK_1_0
            var serializedPerson = BinaryFormatter.Serialize(personOne);
#else
            var binaryFormatter = new BinaryFormatter();
            var serializedPerson = binaryFormatter.Serialize(personOne);
#endif            

            Assert.IsNotNull(serializedPerson);

            Assert.IsTrue(serializedPerson.Length == UnitTestHelper.PersonOneSerialized.Length, "PersonOne serialized data has different length.");

            CollectionAssert.AreEqual(UnitTestHelper.PersonOneSerialized, serializedPerson);
        }

        [TestMethod]
        public void SerializeComplexClassTest()
        {
            // serialize complex class
            var complexClassOne = UnitTestHelper.CreateComplexClassOne();

#if NANOFRAMEWORK_1_0
            var serializedComplexClass = BinaryFormatter.Serialize(complexClassOne);
#else
            var binaryFormatter = new BinaryFormatter();
            var serializedComplexClass = binaryFormatter.Serialize(complexClassOne);
#endif            

            Assert.IsNotNull(serializedComplexClass);

            Assert.IsTrue(serializedComplexClass.Length == UnitTestHelper.ComplexClassOneSerialized.Length, "ComplexClass serialized data has different length.");

            CollectionAssert.AreEqual(UnitTestHelper.ComplexClassOneSerialized, serializedComplexClass);
        }

        [TestMethod]
        public void SerializeArrayListTest()
        {
            // serialize array list
            var arrayListOne = UnitTestHelper.CreateArrayListOne();

#if NANOFRAMEWORK_1_0
            var serializedArrayList = BinaryFormatter.Serialize(arrayListOne);
#else
            var binaryFormatter = new BinaryFormatter();
            var serializedArrayList = binaryFormatter.Serialize(arrayListOne);
#endif

            Assert.IsNotNull(serializedArrayList);

            Assert.IsTrue(serializedArrayList.Length == UnitTestHelper.ArrayListOneSerialized.Length, "ArrayListOne serialized data has different length.");

            CollectionAssert.AreEqual(UnitTestHelper.ArrayListOneSerialized, serializedArrayList);
        }
    }
}
