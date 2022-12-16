//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System.Collections;

#if NANOFRAMEWORK_1_0
using nanoFramework.TestFramework;
using System;
using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nanoFramework.Serialization.Helper;
#endif

namespace nanoFramework.System.Runtime.Serialization.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void DeserializePersonClassTest()
        {
            // serialize person class
            var personOne = UnitTestHelper.CreatePersonOne();

#if NANOFRAMEWORK_1_0
            var newPersonOne = BinaryFormatter.Deserialize(UnitTestHelper.PersonOneSerialized) as Person;
#else
            var binaryFormatter = new BinaryFormatter();
            var newPersonOne = binaryFormatter.Deserialize(UnitTestHelper.PersonOneSerialized) as Person;
#endif            

            Assert.IsNotNull(personOne);

            Assert.AreEqual(personOne.LastName, newPersonOne.LastName);
            Assert.AreEqual(personOne.FirstName, newPersonOne.FirstName);
            Assert.AreEqual(personOne.ID, newPersonOne.ID);
            Assert.AreEqual(personOne.Address, newPersonOne.Address);
            Assert.AreEqual(personOne.Birthday, newPersonOne.Birthday);
            Assert.AreEqual(personOne.Friend.LastName, newPersonOne.Friend.LastName);
            Assert.AreEqual(personOne.Friend.FirstName, newPersonOne.Friend.FirstName);
            Assert.AreEqual(personOne.Friend.ID, newPersonOne.Friend.ID);
            Assert.AreEqual(personOne.Friend.Address, newPersonOne.Friend.Address);
            Assert.AreEqual(personOne.Friend.Birthday, newPersonOne.Friend.Birthday);
        }

        [TestMethod]
        public void DeserializeComplexClassTest()
        {
            // serialize Complex class
            var complexClassOne = UnitTestHelper.CreateComplexClassOne();

#if NANOFRAMEWORK_1_0
            var newComplexClassOne = BinaryFormatter.Deserialize(UnitTestHelper.ComplexClassOneSerialized) as ComplexClass;
#else
            var binaryFormatter = new BinaryFormatter();
            var newComplexClassOne = binaryFormatter.Deserialize(UnitTestHelper.ComplexClassOneSerialized) as ComplexClass;
#endif            

            Assert.IsNotNull(newComplexClassOne);

            Assert.AreEqual(complexClassOne.aInteger, newComplexClassOne.aInteger);
            Assert.AreEqual(complexClassOne.aShort, newComplexClassOne.aShort);
            Assert.AreEqual(complexClassOne.aByte, newComplexClassOne.aByte);
            Assert.AreEqual(complexClassOne.aString, newComplexClassOne.aString);
            Assert.AreEqual(complexClassOne.aFloat, newComplexClassOne.aFloat);
            Assert.AreEqual(complexClassOne.aDouble, newComplexClassOne.aDouble);
            Assert.AreEqual(complexClassOne.aBoolean, newComplexClassOne.aBoolean);
            Assert.AreEqual(complexClassOne.Timestamp, newComplexClassOne.Timestamp);
            Assert.AreEqual(complexClassOne.FixedTimestamp, newComplexClassOne.FixedTimestamp);
            Assert.AreEqual(complexClassOne.nullObject, newComplexClassOne.nullObject);
            Assert.AreEqual(complexClassOne.nanFloat, newComplexClassOne.nanFloat);
            Assert.AreEqual(complexClassOne.nanDouble, newComplexClassOne.nanDouble);

            CollectionAssert.AreEqual(complexClassOne.intArray, newComplexClassOne.intArray);
            CollectionAssert.AreEqual(complexClassOne.shortArray, newComplexClassOne.shortArray);
            CollectionAssert.AreEqual(complexClassOne.byteArray, newComplexClassOne.byteArray);
            CollectionAssert.AreEqual(complexClassOne.stringArray, newComplexClassOne.stringArray);
            CollectionAssert.AreEqual(complexClassOne.floatArray, newComplexClassOne.floatArray);
            CollectionAssert.AreEqual(complexClassOne.doubleArray, newComplexClassOne.doubleArray);

            Assert.AreEqual(complexClassOne.Child.one, newComplexClassOne.Child.one);
            Assert.AreEqual(complexClassOne.Child.two, newComplexClassOne.Child.two);
            Assert.AreEqual(complexClassOne.Child.three, newComplexClassOne.Child.three);

            Assert.AreEqual(complexClassOne.child1.one, newComplexClassOne.child1.one);
            Assert.AreEqual(complexClassOne.child1.two, newComplexClassOne.child1.two);
            Assert.AreEqual(complexClassOne.child1.three, newComplexClassOne.child1.three);
        }

        [TestMethod]
        public void DeserializeArrayListTest()
        {
            // serialize array list
            var arrayListOne = UnitTestHelper.CreateArrayListOne();

#if NANOFRAMEWORK_1_0
            var newArrayListeOne = BinaryFormatter.Deserialize(UnitTestHelper.ArrayListOneSerialized) as ArrayList;
#else
            var binaryFormatter = new BinaryFormatter();
            var newArrayListeOne = binaryFormatter.Deserialize(UnitTestHelper.ArrayListOneSerialized) as ArrayList;
#endif            

            Assert.IsNotNull(newArrayListeOne);

            CollectionAssert.AreEqual(arrayListOne, newArrayListeOne);
        }
    }
}
