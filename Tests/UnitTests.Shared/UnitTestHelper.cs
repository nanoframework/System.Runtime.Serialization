﻿//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Diagnostics;

namespace nanoFramework.System.Runtime.Serialization.Tests
{
    internal class UnitTestHelper
    {
        public static byte[] PersonOneSerialized = new byte[] {
            0x91, 0x74, 0x7A, 0xC0, 0x41, 0x12, 0x9B, 0xDA, 0x1B, 0x80,
            0xD1, 0x1B, 0xD9, 0x7F, 0xE0, 0x6C, 0x8E, 0x22, 0x6D, 0x63,
            0x30, 0x00, 0x00, 0x00, 0x00, 0x06, 0xF4, 0x08, 0x15, 0xA1,
            0x95, 0xB1, 0xB1, 0xBC, 0x15, 0xDD, 0xBD, 0xC9, 0xB1, 0x92,
            0x45, 0xD1, 0xEB, 0x01, 0x03, 0x42, 0x6F, 0x62, 0x05, 0x53,
            0x6D, 0x69, 0x74, 0x68, 0x0B, 0x31, 0x32, 0x33, 0x20, 0x53,
            0x6F, 0x6D, 0x65, 0x20, 0x53, 0x74, 0x81, 0xAC, 0xD4, 0xAA,
            0xC8, 0x2B, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x02, 0xD0, 0x20,
            0x26, 0x86, 0x90, 0x67, 0x06, 0xC6, 0x16, 0xE6, 0x57, 0x40};

        public static Person CreatePersonOne()
        {
            return new Person()
            {
                FirstName = "John",
                LastName = "Doe",
                Birthday = new DateTime(1988, 4, 23),
                ID = 27,
                Address = null,
                ArrayProperty = new string[] { "hello", "world" },
                Friend = new Person()
                {
                    FirstName = "Bob",
                    LastName = "Smith",
                    Birthday = new DateTime(1983, 7, 3),
                    ID = 2,
                    Address = "123 Some St",
                    ArrayProperty = new string[] { "hi", "planet" },
                }
            };
        }

        public static byte[] ComplexClassOneSerialized = new byte[] {
            0x94, 0x08, 0xE6, 0x6E, 0x40, 0x00, 0x00, 0x02, 0x80, 0x3F,
            0x81, 0x42, 0x10, 0x48, 0x1C, 0xDD, 0x1C, 0x9A, 0x5B, 0x99,
            0xCF, 0xE7, 0x81, 0x06, 0x4F, 0xFC, 0xF0, 0x20, 0xC4, 0x9B,
            0xA5, 0xE3, 0x70, 0x2B, 0x69, 0x5A, 0x98, 0x6C, 0x0F, 0x70,
            0x10, 0x3A, 0xC3, 0xF3, 0x62, 0xB9, 0x53, 0x80, 0x1A, 0x0A,
            0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00,
            0x00, 0x0A, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x13,
            0xA0, 0xA0, 0x00, 0x20, 0x00, 0x60, 0x00, 0xA0, 0x00, 0xE0,
            0x01, 0x3A, 0x0A, 0x44, 0x46, 0x48, 0x4A, 0x4D, 0xA0, 0x80,
            0x6E, 0x8E, 0xED, 0xE0, 0x8C, 0xCD, 0xEE, 0xAE, 0x40, 0x6E,
            0x6D, 0x2F, 0x00, 0xAC, 0xAD, 0x2C, 0xED, 0x0E, 0x9A, 0x0A,
            0x7F, 0x19, 0x99, 0x9A, 0x80, 0xA6, 0x66, 0x66, 0x81, 0x60,
            0x00, 0x00, 0x81, 0xEC, 0xCC, 0xCC, 0x82, 0x3C, 0xCC, 0xCD,
            0xA0, 0xA7, 0xFE, 0x3F, 0x34, 0xD6, 0xA1, 0x61, 0xE5, 0x08,
            0x01, 0x58, 0x79, 0x3D, 0xD9, 0x7F, 0x62, 0xC8, 0x02, 0xC8,
            0xB0, 0x9E, 0x98, 0xDC, 0xDB, 0x48, 0x03, 0xE5, 0x01, 0x3A,
            0x92, 0xA3, 0x05, 0x68, 0x04, 0x7A, 0x3F, 0x43, 0x19, 0xE3,
            0xB4, 0x96, 0x1B, 0x57, 0x7B, 0x60, 0x00, 0x00, 0x00, 0x08,
            0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00,
            0x00, 0x05, 0x86, 0xD5, 0xDE, 0xD8, 0x00, 0x00, 0x00, 0xC8,
            0x00, 0x00, 0x01, 0x90, 0x00, 0x00, 0x02, 0x58, 0x00, 0x00,
            0x00, 0x00, 0x7F, 0xE0, 0x00, 0x00, 0x7F, 0xFC, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x05, 0xB2, 0x37, 0xB7, 0x3A, 0x28,
            0x3A, 0xB1, 0x36, 0x34, 0xB9, 0xB4, 0x05, 0xB2, 0x37, 0xB7,
            0x3A, 0x28, 0x3A, 0xB1, 0x36, 0x34, 0xB9, 0xB4, 0x00};

        public static ComplexClass CreateComplexClassOne()
        {
            return new ComplexClass()
            {
                aInteger = 10,
                aShort = 254,
                aByte = 0x05,
                aString = "A string",
                aFloat = 1.2345f,
                aDouble = 1.2345,
                aBoolean = true,
                Timestamp = new DateTime(1910, 10, 10, 07, 30, 59),
                FixedTimestamp = new DateTime(2020, 05, 01, 09, 30, 00),
                intArray = new[] { 1, 3, 5, 7, 9 },
                shortArray = new[] { (short)1, (short)3, (short)5, (short)7, (short)9 },
                byteArray = new[] { (byte)0x22, (byte)0x23, (byte)0x24, (byte)0x25, (byte)0x26 },
                stringArray = new[] { "two", "four", "six", "eight" },
                floatArray = new[] { 1.1f, 3.3f, 5.5f, 7.7f, 9.9f },
                doubleArray = new[] { 1.12345, 3.3456, 5.56789, 7.78910, 9.910111213 },
                child1 = new ChildClass() { one = 1, two = 2, three = 3 },
                Child = new ChildClass() { one = 100, two = 200, three = 300 },
                nullObject = null,
                nanFloat = float.NaN,
                nanDouble = double.NaN,
            };
        }

        public static byte[] ArrayListOneSerialized = new byte[] {
            0xE0, 0x5C, 0xE0, 0xA7, 0x46, 0x57, 0x37, 0x45, 0x37, 0x47,
            0x26, 0x96, 0xE6, 0x7C, 0x80, 0x00, 0x00, 0x02, 0xA2, 0x99,
            0xDE, 0xD9, 0x44, 0x81, 0x74, 0x56, 0xBD, 0xAD, 0xBC, 0xC0,
            0x00, 0x8A, 0x8D, 0xEF, 0x42, 0x40, 0x00, 0x00, 0x00, 0x0E,
            0xE6, 0xB2, 0x80, 0x00};

        public static ArrayList CreateArrayListOne()
        {
            return new ArrayList()
            {
                { "testString" },
                { 42 },
                { null },
                { new DateTime(1933, 2, 11) },
                { TimeSpan.FromSeconds(100) }
            };
        }

        public static void DumpBuffer(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                Debug.Write($"0x{buffer[i].ToString("X2")}, ");
            }

            Debug.WriteLine("");
        }
    }
}
