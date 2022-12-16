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
            var personeOne = UnitTestHelper.CreatePersonOne();

#if NANOFRAMEWORK_1_0
            var newPersoneOne = BinaryFormatter.Deserialize(UnitTestHelper.PersonOneSerialized) as Person;
#else
            var binaryFormatter = new BinaryFormatter();
            var newPersoneOne = binaryFormatter.Deserialize(UnitTestHelper.PersonOneSerialized) as Person;
#endif            

            Assert.IsNotNull(newPersoneOne);
            Assert.AreEqual(newPersoneOne.LastName, personeOne.LastName);
            Assert.AreEqual(newPersoneOne.FirstName, personeOne.FirstName);
            Assert.AreEqual(newPersoneOne.ID, personeOne.ID);
            Assert.AreEqual(newPersoneOne.Address, personeOne.Address);
            Assert.AreEqual(newPersoneOne.Birthday, personeOne.Birthday);
            Assert.AreEqual(newPersoneOne.Friend.LastName, personeOne.Friend.LastName);
            Assert.AreEqual(newPersoneOne.Friend.FirstName, personeOne.Friend.FirstName);
            Assert.AreEqual(newPersoneOne.Friend.ID, personeOne.Friend.ID);
            Assert.AreEqual(newPersoneOne.Friend.Address, personeOne.Friend.Address);
            Assert.AreEqual(newPersoneOne.Friend.Birthday, personeOne.Friend.Birthday);
        }

        [TestMethod]
        public void DeserializeComplexClassTest()
        {
            // serialize Complex class
            var complexeClassOne = UnitTestHelper.CreateComplexClassOne();

#if NANOFRAMEWORK_1_0
            var newComplexeClassOne = BinaryFormatter.Deserialize(UnitTestHelper.ComplexClassOneSerialized) as ComplexClass;
#else
            var binaryFormatter = new BinaryFormatter();
            var newComplexeClassOne = binaryFormatter.Deserialize(UnitTestHelper.ComplexClassOneSerialized) as ComplexClass;
#endif            

            Assert.IsNotNull(newComplexeClassOne);
            Assert.AreEqual(newComplexeClassOne.aInteger, complexeClassOne.aInteger);
            Assert.AreEqual(newComplexeClassOne.aShort, complexeClassOne.aShort);
            Assert.AreEqual(newComplexeClassOne.aByte, complexeClassOne.aByte);
            Assert.AreEqual(newComplexeClassOne.aString, complexeClassOne.aString);
            Assert.AreEqual(newComplexeClassOne.aFloat, complexeClassOne.aFloat);
            Assert.AreEqual(newComplexeClassOne.aDouble, complexeClassOne.aDouble);
            Assert.AreEqual(newComplexeClassOne.aBoolean, complexeClassOne.aBoolean);
            Assert.AreEqual(newComplexeClassOne.Timestamp, complexeClassOne.Timestamp);
            Assert.AreEqual(newComplexeClassOne.FixedTimestamp, complexeClassOne.FixedTimestamp);
            Assert.AreEqual(newComplexeClassOne.nullObject, complexeClassOne.nullObject);
            Assert.AreEqual(newComplexeClassOne.nanFloat, complexeClassOne.nanFloat);
            Assert.AreEqual(newComplexeClassOne.nanDouble, complexeClassOne.nanDouble);

            CollectionAssert.AreEqual(newComplexeClassOne.intArray, complexeClassOne.intArray);
            CollectionAssert.AreEqual(newComplexeClassOne.shortArray, complexeClassOne.shortArray);
            CollectionAssert.AreEqual(newComplexeClassOne.byteArray, complexeClassOne.byteArray);
            CollectionAssert.AreEqual(newComplexeClassOne.stringArray, complexeClassOne.stringArray);
            CollectionAssert.AreEqual(newComplexeClassOne.floatArray, complexeClassOne.floatArray);
            CollectionAssert.AreEqual(newComplexeClassOne.doubleArray, complexeClassOne.doubleArray);

            Assert.AreEqual(newComplexeClassOne.Child.one, complexeClassOne.Child.one);
            Assert.AreEqual(newComplexeClassOne.Child.two, complexeClassOne.Child.two);
            Assert.AreEqual(newComplexeClassOne.Child.three, complexeClassOne.Child.three);

            Assert.AreEqual(newComplexeClassOne.child1.one, complexeClassOne.child1.one);
            Assert.AreEqual(newComplexeClassOne.child1.two, complexeClassOne.child1.two);
            Assert.AreEqual(newComplexeClassOne.child1.three, complexeClassOne.child1.three);
        }

        [TestMethod]
        public void DeserializeArrayListTest()
        {
            // serialize array list
            var personeOne = UnitTestHelper.CreateArrayListOne();

#if NANOFRAMEWORK_1_0
            var newArrayListeOne = BinaryFormatter.Deserialize(UnitTestHelper.ArrayListOneSerialized) as ArrayList;
#else
            var binaryFormatter = new BinaryFormatter();
            var newArrayListeOne = binaryFormatter.Deserialize(UnitTestHelper.ArrayListOneSerialized) as ArrayList;
#endif            

            Assert.IsNotNull(newArrayListeOne);
            CollectionAssert.AreEqual(newArrayListeOne, personeOne);
        }
    }
}
