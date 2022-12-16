//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace nanoFramework.Serialization.Helper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryFormatter"/> class.
    /// </summary>
    /// <remarks>
    /// This helper class is meant to serialize/desearilaize binary data .NET nanoFramework.
    /// </remarks>
    public class BinaryFormatter
    {
        private const int TE_L1 = 2;
        private const uint TE_L1_Null = 0x00000000;
        // N bits for the duplicate id.
        private const uint TE_L1_Duplicate = 0x00000001;
        // 32 bits for the type.
        private const uint TE_L1_Reference = 0x00000002;
        private const uint TE_L1_Other = 0x00000003;

        private const int TE_L2 = 2;
        // 4 bits for the type.
        private const uint TE_L2_Primitive = 0x00000000;
        private const uint TE_L2_Array = 0x00000001;
        private const uint TE_L2_ArrayList = 0x00000002;
        private const uint TE_L2_Other = 0x00000003;

        private const int TE_L3 = 4;
        // 32 bits for the type.
        private const uint TE_L3_Type = 0x00000000;
        // 32 bits for the type, N bits for the signature??
        private const uint TE_L3_Method = 0x00000001;
        // 32 bits for the type, N bits for the signature??
        private const uint TE_L3_Field = 0x00000002;

        private const int TE_ElementType = 4;
        private const int TE_ArrayDepth = 4;

        private const uint INVALID_HASH_ENTRY = 0xffffffff;

        private struct TypeDescriptorBasic
        {
            public const int _Primitive = 0x00000001;
            public const int _Interface = 0x00000002;
            public const int _Class = 0x00000004;
            public const int _ValueType = 0x00000008;
            public const int _Enum = 0x00000010;
            public const int _SemanticMask = 0x0000001F;

            public const int _Array = 0x00010000;
            public const int _ArrayList = 0x00020000;

            public Type _referenceType;
            public ElementType _et;
            public int _flags;

            public void SetType(Type t)
            {
                _referenceType = t;
                _et = ElementType.PELEMENT_TYPE_VOID;
                _flags = 0;

                //
                // Not supported for now.
                //
                if (t == typeof(RuntimeFieldHandle))
                {
                    goto Failure;
                }

                if (t == typeof(RuntimeMethodHandle))
                {
                    goto Failure;
                }

                if (t == typeof(bool))
                {
                    _et = ElementType.PELEMENT_TYPE_BOOLEAN;
                }
                else if (t == typeof(char))
                {
                    _et = ElementType.PELEMENT_TYPE_CHAR;
                }
                else if (t == typeof(sbyte))
                {
                    _et = ElementType.PELEMENT_TYPE_I1;
                }
                else if (t == typeof(byte))
                {
                    _et = ElementType.PELEMENT_TYPE_U1;
                }
                else if (t == typeof(short))
                {
                    _et = ElementType.PELEMENT_TYPE_I2;
                }
                else if (t == typeof(ushort))
                {
                    _et = ElementType.PELEMENT_TYPE_U2;
                }
                else if (t == typeof(int))
                {
                    _et = ElementType.PELEMENT_TYPE_I4;
                }
                else if (t == typeof(uint))
                {
                    _et = ElementType.PELEMENT_TYPE_U4;
                }
                else if (t == typeof(long))
                {
                    _et = ElementType.PELEMENT_TYPE_I8;
                }
                else if (t == typeof(ulong))
                {
                    _et = ElementType.PELEMENT_TYPE_U8;
                }
                else if (t == typeof(float))
                {
                    _et = ElementType.PELEMENT_TYPE_R4;
                }
                else if (t == typeof(double))
                {
                    _et = ElementType.PELEMENT_TYPE_R8;
                }
                else if (t == typeof(string))
                {
                    _et = ElementType.PELEMENT_TYPE_STRING;
                }

                if (_et == ElementType.PELEMENT_TYPE_VOID)
                {
                    if (t == typeof(object)) { _et = ElementType.PELEMENT_TYPE_OBJECT; }
                    else if (t.IsEnum) { _et = ElementType.PELEMENT_TYPE_VALUETYPE; _flags |= _Enum; }
                    else if (t.IsValueType) { _et = ElementType.PELEMENT_TYPE_VALUETYPE; _flags |= _ValueType; }
                    else if (t.IsInterface) { _et = ElementType.PELEMENT_TYPE_CLASS; _flags |= _Interface; }
                    else if (t.IsClass) { _et = ElementType.PELEMENT_TYPE_CLASS; _flags |= _Class; }
                    else
                    {
                        goto Failure;
                    }

                    if (t.IsArray)
                    {
                        _flags |= _Array;
                    }

                    if (t == typeof(ArrayList))
                    {
                        _flags |= _ArrayList;
                    }
                }
                else
                {
                    _flags |= _Primitive;
                }

                return;


            Failure:
                throw Error(string.Format("Unsupported type: {0}", t.FullName));
            }

            public void SetType(ElementType et)
            {
                _et = et;
                _flags = _Primitive;

                switch (_et)
                {
                    case ElementType.PELEMENT_TYPE_BOOLEAN: _referenceType = typeof(bool); break;
                    case ElementType.PELEMENT_TYPE_CHAR: _referenceType = typeof(char); break;
                    case ElementType.PELEMENT_TYPE_I1: _referenceType = typeof(sbyte); break;
                    case ElementType.PELEMENT_TYPE_U1: _referenceType = typeof(byte); break;
                    case ElementType.PELEMENT_TYPE_I2: _referenceType = typeof(short); break;
                    case ElementType.PELEMENT_TYPE_U2: _referenceType = typeof(ushort); break;
                    case ElementType.PELEMENT_TYPE_I4: _referenceType = typeof(int); break;
                    case ElementType.PELEMENT_TYPE_U4: _referenceType = typeof(uint); break;
                    case ElementType.PELEMENT_TYPE_I8: _referenceType = typeof(long); break;
                    case ElementType.PELEMENT_TYPE_U8: _referenceType = typeof(ulong); break;
                    case ElementType.PELEMENT_TYPE_R4: _referenceType = typeof(float); break;
                    case ElementType.PELEMENT_TYPE_R8: _referenceType = typeof(double); break;
                    case ElementType.PELEMENT_TYPE_STRING: _referenceType = typeof(string); break;

                    default: throw Error(string.Format("Invalid primitive type: {0}", _et));
                }
            }

            static public int NumberOfBits(ElementType et)
            {
                switch (et)
                {
                    case ElementType.PELEMENT_TYPE_BOOLEAN: return 1;
                    case ElementType.PELEMENT_TYPE_CHAR: return 16;
                    case ElementType.PELEMENT_TYPE_I1: return 8;
                    case ElementType.PELEMENT_TYPE_U1: return 8;
                    case ElementType.PELEMENT_TYPE_I2: return 16;
                    case ElementType.PELEMENT_TYPE_U2: return 16;
                    case ElementType.PELEMENT_TYPE_I4: return 32;
                    case ElementType.PELEMENT_TYPE_U4: return 32;
                    case ElementType.PELEMENT_TYPE_I8: return 64;
                    case ElementType.PELEMENT_TYPE_U8: return 64;
                    case ElementType.PELEMENT_TYPE_R4: return 32;
                    case ElementType.PELEMENT_TYPE_R8: return 64;
                    case ElementType.PELEMENT_TYPE_STRING: return -1;
                    default: return -2;
                }
            }

            static public bool IsSigned(ElementType et)
            {
                switch (et)
                {
                    case ElementType.PELEMENT_TYPE_BOOLEAN: return false;
                    case ElementType.PELEMENT_TYPE_CHAR: return false;
                    case ElementType.PELEMENT_TYPE_I1: return true;
                    case ElementType.PELEMENT_TYPE_U1: return false;
                    case ElementType.PELEMENT_TYPE_I2: return true;
                    case ElementType.PELEMENT_TYPE_U2: return false;
                    case ElementType.PELEMENT_TYPE_I4: return true;
                    case ElementType.PELEMENT_TYPE_U4: return false;
                    case ElementType.PELEMENT_TYPE_I8: return true;
                    case ElementType.PELEMENT_TYPE_U8: return false;
                    case ElementType.PELEMENT_TYPE_R4: return true;
                    case ElementType.PELEMENT_TYPE_R8: return true;
                    default: return false;
                }
            }

            public Type Type => _referenceType;

            public bool IsPrimitive => (_flags & _Primitive) != 0;
            public bool IsInterface => (_flags & _Interface) != 0;
            public bool IsClass => (_flags & _Class) != 0;
            public bool IsValueType => (_flags & _ValueType) != 0;
            public bool IsEnum => (_flags & _Enum) != 0;

            public bool NeedsSignature => !IsPrimitive && !IsEnum && !IsValueType;

            public bool IsArray => (_flags & _Array) != 0;
            public bool IsArrayList => (_flags & _ArrayList) != 0;

            public bool IsSealed => _referenceType != null && _referenceType.IsSealed;

            static public Exception Error(string msg)
            {
                return new SerializationException(msg);
            }
        }

        private class TypeDescriptor
        {
            public TypeDescriptorBasic _base;
            public int _arrayDepth = 0;
            public TypeDescriptorBasic _arrayElement = new TypeDescriptorBasic();

            public TypeDescriptor(Type t)
            {
                if (t == null)
                {
                    throw new ArgumentException("Unknown type");
                }

                _base.SetType(t);

                if (_base.IsArray)
                {
                    Type sub = t;

                    while (sub.IsArray)
                    {
                        _arrayDepth++;

                        sub = sub.GetElementType();

                        if (sub == null)
                        {
                            throw new ApplicationException("cannot load type");
                        }
                    }

                    _arrayElement.SetType(sub);
                }
            }

            public TypeDescriptor(ElementType et)
            {
                _base.SetType(et);
            }

            public TypeDescriptor(
                ElementType et,
                int depth)
            {
                _base._et = et;
                _base._flags = TypeDescriptorBasic._Array;
                _arrayDepth = depth;
            }

            public Type Type
            {
                get
                {
                    if (_base.IsArray)
                    {
                        Type res = _arrayElement.Type;
                        int depth = _arrayDepth;

                        while (depth-- > 0)
                        {
                            res = Array.CreateInstance(res, 1).GetType();
                        }

                        return res;
                    }

                    return _base.Type;
                }
            }
        }

        private class TypeHandler
        {
            //
            // Type of signatures:
            //
            // 1) NULL
            //
            //      Invalid for NeverNull
            //
            // 2) DUPLICATE <num>
            //
            //      Invalid for Sealed/FixedType + NeverNull
            //
            // 3) PRIMITIVE <et>
            //
            //      <et>      optional for FixedType
            //      PRIMITIVE optional for FixedType + NeverNull
            //
            // 4) REFERENCE <type>
            //
            //      <type>    optional for Sealed/FixedType
            //      REFERENCE optional for Sealed/FixedType + NeverNull
            //
            // 5) ARRAYLIST <length>
            //
            //      <length>  optional for FixedSize
            //      ARRAYLIST optional for FixedType + NeverNull
            //
            // 6) ARRAY <depth> <baseType> <length>
            //
            //      <length>           optional for FixedSize
            //      <depth> <baseType> optional for FixedType
            //      ARRAY              optional for FixedType + NeverNull
            //
            // Always match type if FixedTyped is specified.
            //
            public const int _Signature_Header = 0x01;
            public const int _Signature_Type = 0x02;
            public const int _Signature_Length = 0x04;

            public const int _Action_None = 0;
            public const int _Action_ObjectData = 1;
            public const int _Action_ObjectFields = 2;
            public const int _Action_ObjectElements = 3;

            // ticks at origin is different in .NET nanoFramework
            // for details check the DateTime class source code
            private const long _ticksAtOrigin = 504911232000000000;
            // UTC mask it's alway set
            private const ulong _UTCMask = 0x8000000000000000L;

            private readonly BinaryFormatter _bf;

            public object _value;
            public TypeDescriptor _type;
            public SerializationHintsAttribute _hints;

            public TypeDescriptor _typeExpected;
            public Type _typeForced;

            public TypeHandler(
                BinaryFormatter bf,
                SerializationHintsAttribute hints,
                TypeDescriptor expected)
            {
                _bf = bf;

                _value = null;
                _type = null;
                _hints = hints;

                _typeExpected = expected;
                _typeForced = null;
            }

            public void SetValue(object v)
            {
                if (v != null)
                {
                    Type t = v.GetType();

                    if (t.IsEnum
                        || t.IsPrimitive
                        || (t.Attributes & TypeAttributes.Serializable) != 0)
                    {
                        _value = v;
                        _type = new TypeDescriptor(v.GetType());
                    }
                }
            }

            public int SignatureRequirements()
            {
                int res = _Signature_Header | _Signature_Type | _Signature_Length;

                if (Hints_ArraySize != 0)
                {
                    res &= ~_Signature_Length;
                }

                _typeForced = null;

                if (_typeExpected != null)
                {
                    if (!_typeExpected._base.NeedsSignature)
                    {
                        res = 0;
                    }
                    else
                    {
                        if (Hints_FixedType)
                        {
                            res &= ~_Signature_Type;
                        }
                        else
                        {
                            TypeDescriptorBasic td;

                            if (_typeExpected._base.IsArray)
                            {
                                td = _typeExpected._arrayElement;
                            }
                            else
                            {
                                td = _typeExpected._base;
                            }

                            if (td.IsSealed)
                            {
                                res &= ~_Signature_Type;
                            }
                        }
                    }
                }

                if ((res & _Signature_Type) == 0)
                {
                    _typeForced = _typeExpected.Type;

                    if (Hints_PointerNeverNull)
                    {
                        res &= ~_Signature_Header;
                    }
                }

                return res;
            }

            public bool Hints_PointerNeverNull => (_hints != null && (_hints.Options & SerializationOptions.PointerNeverNull) != 0);

            public bool Hints_FixedType => (_hints != null && (_hints.Options & SerializationOptions.FixedType) != 0);

            public int Hints_ArraySize => _hints == null ? 0 : _hints.ArraySize;

            public int Hints_BitPacked => _hints == null ? 0 : _hints.BitPacked;
            public long Hints_RangeBias => _hints == null ? 0 : _hints.RangeBias;
            public ulong Hints_Scale => _hints == null ? 0 : _hints.Scale;

            public int EmitSignature()
            {
                int mask = SignatureRequirements();

                if ((mask
                    & _Signature_Type) == 0)
                {
                    if (_value != null
                        && _value.GetType() != _typeForced)
                    {
                        if (_typeForced == typeof(Type)
                            && _value.GetType().IsSubclassOf(_typeForced))
                        {
                        }
                        else
                        {
                            throw TypeDescriptorBasic.Error("FixedType violation");
                        }
                    }
                }

                if (_value == null)
                {
                    if (mask == 0)
                    {
                        //
                        // Special case for null strings (strings don't emit an hash): send a string of length -1.
                        //
                        if (_typeExpected != null
                            && _typeExpected._base._et == ElementType.PELEMENT_TYPE_STRING)
                        {
                            WriteLine("NULL STRING");

                            _bf.WriteCompressedUnsigned(0xFFFFFFFF);

                            return _Action_None;
                        }

                        throw TypeDescriptorBasic.Error("NoSignature violation");
                    }

                    if (Hints_PointerNeverNull)
                    {
                        throw TypeDescriptorBasic.Error("PointerNeverNull violation");
                    }

                    WriteLine("NULL Pointer");

                    _bf.WriteBits(TE_L1_Null, TE_L1);

                    return _Action_None;
                }

                int idx = _bf.SearchDuplicate(_value);

                if (idx != -1)
                {
                    //
                    // No duplicates allowed for fixed-type objects.
                    //
                    if ((mask
                        & _Signature_Header) == 0)
                    {
                        throw TypeDescriptorBasic.Error("No duplicates for FixedType+PointerNeverNull!");
                    }

                    WriteLine("Duplicate: {0}", idx);

                    _bf.WriteBits(TE_L1_Duplicate, TE_L1);
                    _bf.WriteCompressedUnsigned((uint)idx);

                    return _Action_None;
                }

                EmitSignature_Inner(mask, _type._base, _type, _value);

                return _Action_ObjectData;
            }

            public void EmitSignature_Inner(
                int mask,
                TypeDescriptorBasic td,
                TypeDescriptor tdBase,
                object v)
            {
                int sizeReal = -1;

                if (td.Type.IsSubclassOf(typeof(Type)))
                {
                    Type t = (Type)v;

                    WriteLine("Reference: {0} {1}", mask, t.FullName);

                    if ((mask
                        & _Signature_Header) != 0)
                    {
                        _bf.WriteBits(TE_L1_Other, TE_L1);
                        _bf.WriteBits(TE_L2_Other, TE_L2);
                        _bf.WriteBits(TE_L3_Type, TE_L3);
                    }
                }
                else if (td.IsPrimitive)
                {
                    WriteLine("Primitive: {0} {1}", mask, td._et);

                    if ((mask & _Signature_Header)
                        != 0)
                    {
                        _bf.WriteBits(TE_L1_Other, TE_L1);
                        _bf.WriteBits(TE_L2_Primitive, TE_L2);
                    }

                    if ((mask & _Signature_Type) != 0)
                    {
                        _bf.WriteBits((uint)td._et, TE_ElementType);
                    }
                }
                else if (td.IsArray)
                {
                    Array arr = (Array)v;

                    WriteLine("Array: Depth {0} {1}", mask, tdBase._arrayDepth);

                    if ((mask & _Signature_Header)
                        != 0)
                    {
                        _bf.WriteBits(TE_L1_Other, TE_L1);
                        _bf.WriteBits(TE_L2_Array, TE_L2);
                    }

                    if ((mask & _Signature_Type) != 0)
                    {
                        _bf.WriteBits((uint)tdBase._arrayDepth, TE_ArrayDepth);

                        EmitSignature_Inner(_Signature_Header | _Signature_Type, tdBase._arrayElement, null, null);
                    }

                    WriteLine("Array: Size {0}", arr.Length);

                    sizeReal = arr.Length;
                }
                else if (td.IsArrayList
                    && v != null)
                {
                    ArrayList lst = (ArrayList)v;

                    WriteLine("ArrayList: Size {0}", lst.Count);

                    if ((mask & _Signature_Header)
                        != 0)
                    {
                        _bf.WriteBits(TE_L1_Other, TE_L1);
                        _bf.WriteBits(TE_L2_ArrayList, TE_L2);
                    }

                    sizeReal = lst.Count;
                }
                else
                {
                    Type t;

                    if (v != null)
                    {
                        t = v.GetType();
                    }
                    else
                    {
                        t = td.Type;
                    }

                    WriteLine($"Reference: {mask} {t.FullName}");

                    if ((mask & _Signature_Header)
                        != 0)
                    {
                        _bf.WriteBits(TE_L1_Reference, TE_L1);
                    }

                    if ((mask & _Signature_Type) != 0)
                    {
                        _bf.WriteType(t);
                    }
                }

                if (sizeReal != -1)
                {
                    if ((mask & _Signature_Length) != 0)
                    {
                        int bitsMax = Hints_BitPacked;

                        if (bitsMax != 0)
                        {
                            if (sizeReal >= (1 << bitsMax))
                            {
                                throw TypeDescriptorBasic.Error(string.Format("Array size outside range: Bits={0}", bitsMax));
                            }

                            _bf.WriteBits((uint)sizeReal, bitsMax);
                        }
                        else
                        {
                            _bf.WriteCompressedUnsigned((uint)sizeReal);
                        }
                    }
                    else
                    {
                        int sizeExpected = Hints_ArraySize;

                        if (sizeExpected > 0 && sizeExpected != sizeReal)
                        {
                            throw TypeDescriptorBasic.Error(string.Format("ArraySize violation: (Expected: {0} Got:{1})", sizeReal, sizeExpected));
                        }
                    }
                }
            }

            public int ReadSignature()
            {
                int mask = SignatureRequirements();

                _value = null;
                _type = (_typeForced == null) ? null : new TypeDescriptor(_typeForced);

                if ((mask & _Signature_Header) != 0)
                {
                    uint levelOne;
                    uint levelTwo;
                    uint levelThree;

                    levelOne = _bf.ReadBits(TE_L1);

                    if (levelOne == TE_L1_Null)
                    {
                        if (Hints_PointerNeverNull)
                        {
                            throw TypeDescriptorBasic.Error("PointerNeverNull violation");
                        }

                        WriteLine("NULL Pointer");

                        return _Action_None;
                    }
                    else if (levelOne == TE_L1_Duplicate)
                    {
                        int idx = (int)_bf.ReadCompressedUnsigned();

                        _value = _bf.GetDuplicate(idx);
                        _type = new TypeDescriptor(_value.GetType());

                        WriteLine("Duplicate: {0}", idx);

                        return _Action_None;
                    }
                    else if (levelOne == TE_L1_Reference)
                    {
                        if ((mask & _Signature_Type) != 0)
                        {
                            _type = new TypeDescriptor(_bf.ReadType());
                        }

                        WriteLine("Reference: {0}", _type.Type.FullName);
                    }
                    else
                    {
                        levelTwo = _bf.ReadBits(TE_L2);

                        if (levelTwo == TE_L2_Primitive)
                        {
                            if ((mask & _Signature_Type) != 0)
                            {
                                _type = new TypeDescriptor((ElementType)_bf.ReadBits(TE_ElementType));
                            }

                            WriteLine("Primitive: {0}", _type._base._et);
                        }
                        else if (levelTwo == TE_L2_Array)
                        {
                            if ((mask & _Signature_Type) != 0)
                            {
                                _type = new TypeDescriptor(ElementType.PELEMENT_TYPE_CLASS, (int)_bf.ReadBits(TE_ArrayDepth));

                                WriteLine("Array: Depth {0}", _type._arrayDepth);

                                levelOne = _bf.ReadBits(TE_L1);

                                if (levelOne == TE_L1_Reference)
                                {
                                    _type._arrayElement.SetType(_bf.ReadType());
                                }
                                else if (levelOne == TE_L1_Other)
                                {
                                    levelTwo = _bf.ReadBits(TE_L2);

                                    if (levelTwo == TE_L2_Primitive)
                                    {
                                        _type._arrayElement.SetType((ElementType)_bf.ReadBits(TE_ElementType));
                                    }
                                    else
                                    {
                                        throw TypeDescriptorBasic.Error(string.Format("Unexpected Level2 value: {0}", levelTwo));
                                    }
                                }
                                else
                                {
                                    throw TypeDescriptorBasic.Error(string.Format("Unexpected Level1 value: {0}", levelOne));
                                }
                            }
                        }
                        else if (levelTwo == TE_L2_ArrayList)
                        {
                            if ((mask & _Signature_Type) != 0)
                            {
                                _type = new TypeDescriptor(typeof(ArrayList));
                            }
                        }
                        else if (levelTwo == TE_L2_Other)
                        {
                            levelThree = _bf.ReadBits(TE_L3);
                            if (levelThree == TE_L3_Type)
                            {
                                _type = new TypeDescriptor(typeof(Type));
                            }
                            else
                            {
                                throw TypeDescriptorBasic.Error(string.Format("Unexpected Level3 value: {0}", levelThree));
                            }
                        }
                    }
                }

                if (_type._base.IsArray || _type._base.IsArrayList)
                {
                    int len;

                    if ((mask & _Signature_Length) != 0)
                    {
                        int bitsMax = Hints_BitPacked;

                        if (bitsMax != 0)
                        {
                            len = (int)_bf.ReadBits(bitsMax);
                        }
                        else
                        {
                            len = (int)_bf.ReadCompressedUnsigned();
                        }
                    }
                    else
                    {
                        len = Hints_ArraySize;

                        if (len == -1)
                        {
                            int bits = TypeDescriptorBasic.NumberOfBits(_type._arrayElement._et);

                            if (bits < 0)
                            {
                                throw TypeDescriptorBasic.Error("Only primitive types allowed for ArraySize = -1");
                            }

                            len = _bf.BitsAvailable() / bits;
                        }
                    }

                    if (_type._base.IsArrayList)
                    {
                        _value = new ArrayList(len);
                    }
                    else
                    {
                        _value = Array.CreateInstance(_type.Type.GetElementType(), len);

                        WriteLine("Array: Size {0}", ((Array)_value).Length);
                    }
                }
                else
                {
                    if (_type.Type == typeof(Type))
                    {
                        _value = _bf.ReadType();

                        return _Action_None;
                    }

                    if (_type.Type == typeof(string))
                    {
                        _value = null;
                    }
                    else
                    {
                        _value = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(_type.Type);
                    }
                }

                return _Action_ObjectData;
            }

            public int TrackObject()
            {
                if (_type._base.IsArray
                    || _type._base.IsArrayList)
                {
                    _bf.TrackDuplicate(_value);

                    return _Action_ObjectElements;
                }
                else
                {
                    if (_typeExpected == null
                        || _typeExpected._base.NeedsSignature)
                    {
                        _bf.TrackDuplicate(_value);
                    }

                    return _Action_ObjectFields;
                }
            }

            public int EmitValue()
            {
                if (_type.Type.IsSubclassOf(typeof(Type)))
                {
                    _bf.WriteType((Type)_value);

                    return _Action_None;
                }

                ulong val;
                int bits;
                bool fSigned;

                if (_type._base.IsPrimitive)
                {
                    switch (_type._base._et)
                    {
                        case ElementType.PELEMENT_TYPE_BOOLEAN: val = (bool)_value ? 1UL : 0UL; break;
                        case ElementType.PELEMENT_TYPE_CHAR: val = (char)_value; break;
                        case ElementType.PELEMENT_TYPE_I1: val = (ulong)(sbyte)_value; break;
                        case ElementType.PELEMENT_TYPE_U1: val = (byte)_value; break;
                        case ElementType.PELEMENT_TYPE_I2: val = (ulong)(short)_value; break;
                        case ElementType.PELEMENT_TYPE_U2: val = (ushort)_value; break;
                        case ElementType.PELEMENT_TYPE_I4: val = (ulong)(int)_value; break;
                        case ElementType.PELEMENT_TYPE_U4: val = (uint)_value; break;
                        case ElementType.PELEMENT_TYPE_I8: val = (ulong)(long)_value; break;
                        case ElementType.PELEMENT_TYPE_U8: val = (ulong)_value; break;
                        case ElementType.PELEMENT_TYPE_R4: val = BytesFromFloat((float)_value); break;
                        case ElementType.PELEMENT_TYPE_R8: val = BytesFromDouble((double)_value); break;

                        case ElementType.PELEMENT_TYPE_STRING:
                            {
                                byte[] buf = Encoding.UTF8.GetBytes((string)_value);

                                _bf.WriteCompressedUnsigned((uint)buf.Length);
                                _bf.WriteArray(buf, 0, buf.Length);

                                WriteLine("Value: STRING {0}", _value);

                                return _Action_None;
                            }

                        default:
                            throw TypeDescriptorBasic.Error("Bad primitive");
                    }

                    bits = TypeDescriptorBasic.NumberOfBits(_type._base._et);
                    fSigned = TypeDescriptorBasic.IsSigned(_type._base._et);
                }
                else if (_value is DateTime)
                {
                    val = (ulong)((DateTime)_value).Ticks;
                    // subtract ticks at origin and set UTC mask to match .NET nanoFramework ticks
                    val -= _ticksAtOrigin;
                    val |= _UTCMask;
                    bits = 64;
                    fSigned = false;
                }
                else if (_value is TimeSpan)
                {
                    val = (ulong)((TimeSpan)_value).Ticks;
                    bits = 64;
                    fSigned = true;
                }
                else
                {
                    return TrackObject();
                }

                WriteLine("Value: NUM {0}", val);

                if (Hints_BitPacked != 0)
                {
                    bits = Hints_BitPacked;
                }

                bool fValid = true;

                val -= (ulong)Hints_RangeBias;

                if (fSigned)
                {
                    long valS = (long)val;

                    if (Hints_Scale != 0)
                    {
                        valS /= (long)Hints_Scale;
                    }

                    if (bits != 64)
                    {
                        if (_type._base._et
                            == ElementType.PELEMENT_TYPE_R4)
                        {
                            // special handling if it's a float
                            // anything goes as long as it's inside the low 32 bits
                            if (((ulong)valS & 0xFFFFFFFF_00000000) > 0)
                            {
                                fValid = false;
                            }
                        }
                        else
                        {
                            // all the rest
                            long maxVal = (1L << (bits - 1)) - 1;

                            fValid = (valS <= maxVal) && (valS >= -maxVal - 1);
                        }
                    }

                    val = (ulong)valS;
                }
                else
                {
                    ulong valU = val;

                    if (Hints_Scale != 0)
                    {
                        valU /= Hints_Scale;
                    }

                    if (bits != 64)
                    {
                        ulong maxVal = (1UL << bits) - 1;

                        fValid = (valU <= maxVal);
                    }

                    val = valU;
                }

                if (!fValid)
                {
                    throw TypeDescriptorBasic.Error(string.Format("Value outside range: Bits={0} Bias={1} Scale={2}", bits, Hints_RangeBias, Hints_Scale));
                }

                _bf.WriteBitsLong(val, bits);

                return _Action_None;
            }

            public int ReadValue()
            {
                ulong val;
                int bits;
                bool fSigned;

                if (_type._base.IsPrimitive)
                {
                    if (_type._base._et
                        == ElementType.PELEMENT_TYPE_STRING)
                    {
                        uint len = _bf.ReadCompressedUnsigned();

                        if (len == 0xFFFFFFFF)
                        {
                            _value = null;
                        }
                        else
                        {
                            byte[] buf = new byte[len];

                            _bf.ReadArray(buf, 0, (int)len);

                            _value = Encoding.UTF8.GetString(buf);
                        }

                        WriteLine("Value: STRING {0}", _value);

                        return _Action_None;
                    }

                    bits = TypeDescriptorBasic.NumberOfBits(_type._base._et);
                    fSigned = TypeDescriptorBasic.IsSigned(_type._base._et);

                    if (bits < 0)
                    {
                        throw TypeDescriptorBasic.Error("Bad primitive");
                    }
                }
                else if (_type.Type == typeof(DateTime))
                {
                    bits = 64;
                    fSigned = false;
                }
                else if (_type.Type == typeof(TimeSpan))
                {
                    bits = 64;
                    fSigned = true;
                }
                else
                {
                    return TrackObject();
                }

                if (Hints_BitPacked != 0)
                {
                    bits = Hints_BitPacked;
                }

                val = _bf.ReadBitsLong(bits);

                if (fSigned)
                {
                    long valS;

                    if (bits != 64)
                    {
                        valS = (long)(val << (64 - bits));
                        val = (ulong)(valS >> (64 - bits));
                    }

                    valS = (long)val;

                    if (Hints_Scale != 0)
                    {
                        valS *= (long)Hints_Scale;
                    }

                    val = (ulong)valS;
                }
                else
                {
                    ulong valU;

                    if (bits != 64)
                    {
                        valU = val << (64 - bits);
                        val = valU >> (64 - bits);
                    }

                    valU = val;

                    if (Hints_Scale != 0)
                    {
                        valU *= Hints_Scale;
                    }

                    val = valU;
                }

                val += (ulong)Hints_RangeBias;

                WriteLine("Value: NUM {0}", val);

                if (_type.Type == typeof(DateTime))
                {
                    // add at origin and clear UTC mask to transform from .NET nanoFramework ticks
                    val += _ticksAtOrigin;
                    val &= ~_UTCMask;

                    _value = new DateTime((long)val, DateTimeKind.Utc);
                }
                else if (_type.Type == typeof(TimeSpan))
                {
                    _value = new TimeSpan((long)val);
                }
                else
                {
                    switch (_type._base._et)
                    {
                        case ElementType.PELEMENT_TYPE_BOOLEAN: _value = val != 0; break;
                        case ElementType.PELEMENT_TYPE_CHAR: _value = (char)val; break;
                        case ElementType.PELEMENT_TYPE_I1: _value = (sbyte)val; break;
                        case ElementType.PELEMENT_TYPE_U1: _value = (byte)val; break;
                        case ElementType.PELEMENT_TYPE_I2: _value = (short)val; break;
                        case ElementType.PELEMENT_TYPE_U2: _value = (ushort)val; break;
                        case ElementType.PELEMENT_TYPE_I4: _value = (int)val; break;
                        case ElementType.PELEMENT_TYPE_U4: _value = (uint)val; break;
                        case ElementType.PELEMENT_TYPE_I8: _value = (long)val; break;
                        case ElementType.PELEMENT_TYPE_U8: _value = val; break;
                        case ElementType.PELEMENT_TYPE_R4: _value = FloatFromBytes(val); break;
                        case ElementType.PELEMENT_TYPE_R8: _value = DoubleFromBytes(val); break;
                    }
                }

                return _Action_None;
            }

            unsafe public float FloatFromBytes(ulong val)
            {
                float ret = 0.0f;

                // assume always floating point
                uint val2 = (uint)val;
                uint* ptr = &val2;
                ret = *(float*)ptr;

                return ret;
            }

            unsafe public double DoubleFromBytes(ulong val)
            {
                double ret = 0.0;

                // assume always floating point
                ulong* ptr = &val;
                ret = *(double*)ptr;

                return ret;
            }

            unsafe public ulong BytesFromFloat(float val)
            {
                ulong ret = 0;

                // assume always floating point
                float* ptr = &val;
                ret = *(uint*)ptr;

                return ret;
            }

            unsafe public ulong BytesFromDouble(double val)
            {
                ulong ret = 0;

                // assume always floating point
                double* ptr = &val;
                ret = *(ulong*)ptr;

                return ret;
            }
        }

        private class State
        {
            private readonly BinaryFormatter _parent;
            private State _next = null;
            private State _prev = null;

            private bool _value_NeedProcessing = true;
            private readonly TypeHandler _value;

            private bool _fields_NeedProcessing = false;
            private Type _fields_CurrentClass = null;
            private FieldInfo[] _fields_Fields = null;
            private int _fields_CurrentField = 0;

            private bool _array_NeedProcessing = false;
            private Type _array_ExpectedType = null;
            private int _array_CurrentPos = 0;
            private int _array_LastPos = 0;

            public State(
                BinaryFormatter parent,
                SerializationHintsAttribute hints,
                Type t)
            {
                WriteLine("New State: {0}", t != null ? t.FullName : "<null>");

                TypeDescriptor td = (t != null) ? new TypeDescriptor(t) : null;

                _parent = parent;
                _value = new TypeHandler(parent, hints, td);
            }

            private State CreateInstance(
                SerializationHintsAttribute hints,
                Type t)
            {
                State next = new(
                    _parent,
                    hints,
                    t);

                _next = next;
                next._prev = this;

                return next;
            }

            private void GetValue()
            {
                State prev = _prev;
                object o = null;

                if (prev == null)
                {
                    o = _parent._value;
                }
                else
                {
                    if (prev._fields_NeedProcessing)
                    {
                        o = prev._fields_Fields[prev._fields_CurrentField - 1].GetValue(prev._value._value);
                    }

                    if (prev._array_NeedProcessing)
                    {
                        if (prev._value._type._base.IsArrayList)
                        {
                            ArrayList lst = (ArrayList)prev._value._value;

                            o = lst[prev._array_CurrentPos - 1];
                        }
                        else
                        {
                            Array arr = (Array)prev._value._value;

                            o = arr.GetValue(prev._array_CurrentPos - 1);
                        }
                    }
                }

                WriteLine("New Object: {0}", o != null ? o.GetType().FullName : "<null>");

                _value.SetValue(o);
            }

            private State SetValueAndDestroyInstance()
            {
                State prev = _prev;

                if (prev == null)
                {
                    if (_parent._fDeserialize)
                    {
                        _parent._value = _value._value;
                    }
                }
                else
                {
                    if (_parent._fDeserialize)
                    {
                        object o = _value._value;

                        if (prev._fields_NeedProcessing)
                        {
                            prev._fields_Fields[prev._fields_CurrentField - 1].SetValue(prev._value._value, o);
                        }

                        if (prev._array_NeedProcessing)
                        {
                            if (prev._value._type._base.IsArrayList)
                            {
                                ArrayList lst = (ArrayList)prev._value._value;

                                lst.Add(o);
                            }
                            else
                            {
                                Array arr = (Array)prev._value._value;

                                arr.SetValue(o, prev._array_CurrentPos - 1);
                            }
                        }
                    }

                    prev._next = null;
                }

                return prev;
            }

            public State Advance()
            {
                if (_value_NeedProcessing)
                {
                    int res;

                    _value_NeedProcessing = false;

                    if (_parent._fDeserialize)
                    {
                        res = _value.ReadSignature();
                    }
                    else
                    {
                        GetValue();

                        res = _value.EmitSignature();
                    }

                    if (res != TypeHandler._Action_None)
                    {
                        object o;

                        if (_parent._fDeserialize)
                        {
                            res = _value.ReadValue();
                        }
                        else
                        {

                            res = _value.EmitValue();
                        }

                        o = _value._value;

                        switch (res)
                        {
                            case TypeHandler._Action_None:
                                break;

                            case TypeHandler._Action_ObjectFields:
                                {
                                    _fields_NeedProcessing = true;
                                    _fields_CurrentClass = o.GetType();
                                    _fields_Fields = null;
                                    break;
                                }

                            case TypeHandler._Action_ObjectElements:
                                {
                                    _array_NeedProcessing = true;
                                    _array_CurrentPos = 0;

                                    if (o is ArrayList)
                                    {
                                        ArrayList lst = (ArrayList)o;

                                        _array_ExpectedType = null;
                                        _array_LastPos = _parent._fDeserialize ? lst.Capacity : lst.Count;
                                    }
                                    else
                                    {
                                        Array arr = (Array)o;

                                        _array_ExpectedType = arr.GetType().GetElementType();
                                        _array_LastPos = arr.Length;
                                    }

                                    break;
                                }

                            default:
                                throw new System.Runtime.Serialization.SerializationException();
                        }
                    }
                }

                if (_fields_NeedProcessing)
                {
                    return AdvanceToTheNextField();
                }

                if (_array_NeedProcessing)
                {
                    return AdvanceToTheNextElement();
                }

                return SetValueAndDestroyInstance();
            }

            private State AdvanceToTheNextField()
            {
                while (_fields_CurrentClass != null)
                {
                    if (_fields_Fields == null)
                    {
                        _fields_Fields = _fields_CurrentClass.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        _fields_CurrentField = 0;
                    }

                    if (_fields_CurrentField < _fields_Fields.Length)
                    {
                        FieldInfo f = _fields_Fields[_fields_CurrentField++];

                        if ((f.Attributes & FieldAttributes.NotSerialized) == 0)
                        {
                            SerializationHintsAttribute hints;

                            if (_value._type._base.IsEnum)
                            {
                                hints = _value._hints;
                            }
                            else
                            {
                                hints = GetHints(f);
                            }

                            WriteLine("Field: {0} {1}", f.Name, f.FieldType);

                            return CreateInstance(hints, f.FieldType);
                        }
                    }
                    else
                    {
                        _fields_CurrentClass = _fields_CurrentClass.BaseType;
                        _fields_Fields = null;
                    }
                }

                return SetValueAndDestroyInstance();
            }

            private State AdvanceToTheNextElement()
            {
                if (_array_CurrentPos++ < _array_LastPos)
                {
                    SerializationHintsAttribute hints;

                    if (_value._hints != null && (_value._hints.Options & (SerializationOptions.FixedType | SerializationOptions.PointerNeverNull)) != 0)
                    {
                        hints = new SerializationHintsAttribute
                        {
                            BitPacked = 0,
                            Options = _value._hints.Options & (SerializationOptions.FixedType | SerializationOptions.PointerNeverNull)
                        };
                    }
                    else
                    {
                        hints = null;
                    }

                    return CreateInstance(hints, _array_ExpectedType);
                }

                return SetValueAndDestroyInstance();
            }
        }

        private bool _fOnlySerializableObjects = false;
        private Hashtable _htLookupType = new Hashtable();
        private Hashtable _htLookupHash = new Hashtable();
        private ArrayList _lstProcessedAssemblies = new ArrayList();

        private BitStream _stream;
        private State _first;
        private object _value;
        private bool _fDeserialize;

        private readonly Hashtable _htDuplicates = new();
        private readonly ArrayList _lstObjects = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormatter"/> class.
        /// </summary>
        public BinaryFormatter()
        {

        }

        private void Initialize(
            Type t,
            bool fDeserialize)
        {
            _first = new State(this, BuildHints(t), t);
            _fDeserialize = fDeserialize;
            _htDuplicates.Clear();
            LstObjects.Clear();

            if (fDeserialize)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        LookupHash(type);
                    }
                }
            }
        }

        private void InitializeForSerialization(
            Type t,
            object o)
        {
            _stream = new BitStream();
            _value = o;
            Initialize(t, false);
        }

        private void InitializeForDeserialization(
            Type t,
            byte[] data,
            int pos,
            int len)
        {
            _stream = new BitStream(data, pos, len);
            _value = null;
            Initialize(t, true);
        }

        private SerializationHintsAttribute BuildHints(Type t)
        {
            if (t == null)
            {
                return null;
            }

            SerializationHintsAttribute hints = new SerializationHintsAttribute
            {
                Options = SerializationOptions.FixedType | SerializationOptions.PointerNeverNull
            };

            return hints;
        }

        private ArrayList LstObjects => _lstObjects;

        private static void ConvertObject(
            object dst,
            object src)
        {
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fi = src.GetType().GetFields(bindingFlags);

            for (int iField = 0; iField < fi.Length; iField++)
            {
                FieldInfo fieldSrc = fi[iField];

                FieldInfo fieldDst = dst.GetType().GetField(fieldSrc.Name, bindingFlags);

                if (fieldSrc.FieldType == typeof(Type))
                {
                    continue;
                }

                if (fieldDst == null)
                {
                    Debug.Assert(false, "ConvertObject Incompatibility");
                    continue;
                }

                object valSrc = fieldSrc.GetValue(src);
                object valDst = null;

                if ((fieldSrc.FieldType.IsValueType || fieldSrc.FieldType.IsEnum) && (!fieldSrc.FieldType.IsPrimitive))
                {
                    valDst = fieldDst.GetValue(dst);

                    ConvertObject(valDst, valSrc);
                }
                else
                {
                    valDst = valSrc;
                }

                fieldDst.SetValue(dst, valDst);
            }
        }

        static private SerializationHintsAttribute GetHints(MemberInfo mi)
        {
            SerializationHintsAttribute hintsDst = null;

            if (mi != null)
            {
                object[] lst = mi.GetCustomAttributes(false);
                bool fFoundSerializationHints = false;

                for (int iAttr = 0; iAttr < lst.Length; iAttr++)
                {
                    object hintsSrc = lst[iAttr];

                    if (hintsSrc.GetType().FullName == "System.Runtime.Serialization.SerializationHintsAttribute")
                    {
                        if (fFoundSerializationHints)
                        {
                            throw new NotSupportedException("Only one attribute per type!");
                        }

                        fFoundSerializationHints = true;

                        hintsDst = new SerializationHintsAttribute();

                        ConvertObject(hintsDst, hintsSrc);
                    }
                }
            }

            return hintsDst;
        }

        /// <summary>
        /// Serializes the object, or graph of objects with the specified top (root), to a byte array.
        /// </summary>
        /// <param name="graph">The object at the root of the graph to serialize.</param>
        /// <returns>A byte array with the serialized graph.</returns>
        /// <remarks>This implementation is specific to serialized data in binary format that can be used in .NET nanoFramework applications.</remarks>
        /// <exception cref="SerializationException">An error has occurred during serialization.</exception>
        public byte[] Serialize(object graph)
        {
            return Serialize(null, graph);
        }

        /// <summary>
        /// Deserializes the specified byte array into an object graph.
        /// </summary>
        /// <param name="buffer">The byte array from which to deserialize the object graph.</param>
        /// <returns>The top (root) of the object graph.</returns>
        /// <remarks>This implementation is specific to serialize binary data generated by .NET nanoFramework <see cref="BinaryFormatter"/>.</remarks>
        /// <exception cref="SerializationException">An error occurred while deserializing an object from the byte array.</exception>
        public object Deserialize(byte[] buffer) => Deserialize(
            null,
            buffer,
            0,
            buffer.Length);

        private object Deserialize(
            Type t,
            byte[] v)
        {
            return Deserialize(t, v, 0, v.Length);
        }

        private byte[] Serialize(
            Type t,
            object o)
        {
            InitializeForSerialization(t, o);

            State current = _first;

            while (current != null)
            {
                current = current.Advance();
            }

            return _stream.ToArray();
        }

        private object Deserialize(
            Type t,
            byte[] v,
            int pos,
            int len)
        {
            InitializeForDeserialization(t, v, pos, len);

            State current = _first;

            while (current != null)
            {
                current = current.Advance();
            }

            return _value;
        }

        private void TrackDuplicate(object o)
        {
            if (o is Type)
            {
                return;
            }

            _htDuplicates[o] = _htDuplicates.Count;
            LstObjects.Add(o);
        }

        private int SearchDuplicate(object o)
        {
            if (o is Type)
            {
                return -1;
            }

            if (!_htDuplicates.Contains(o))
            {
                return -1;
            }

            return (int)_htDuplicates[o];
        }

        private object GetDuplicate(int idx)
        {
            return LstObjects[idx];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal bool AddType(Type t)
        {
            if (t == null)
            {
                return true;
            }

            if (_htLookupHash.Contains(t))
            {
                return true;
            }

            //Ok, this is really evil. But it's this or 800 exceptions trying to load assemblies through BF.Initialize

            //This is for dealing with generics                        
            if (t.FullName.IndexOf('`') >= 0)
            {
                return false;
            }

            //Lots of autogenerated code to ignore
            if (t.FullName.StartsWith("Microsoft.Internal.Deployment")
                || t.FullName == "ThisAssembly"
                || t.FullName == "AssemblyRef"
                || t.FullName.StartsWith("<PrivateImplementationDetails>"))
            {
                return false;
            }

            AddTypeCore(t);

            AddType(t.GetElementType());
            AddType(t.BaseType);

            foreach (FieldInfo f in t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if ((f.Attributes & FieldAttributes.NotSerialized) != 0)
                {
                    continue;
                }

                AddType(f.FieldType);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void AddTypeCore(Type t)
        {
            try
            {
                uint hash = 0;

                //
                // Handling of special cases.
                //
                if (t == typeof(ArrayList))
                {
                    hash = 0xEDDD427F;
                }
                else
                {
                    hash = ComputeHashForType(t, 0);
                }

                Type tExists = _htLookupType[hash] as Type;
                if (tExists != null)
                {
                    string error = string.Format("Hash conflict: 0x{0:x8} {1}", hash, t.AssemblyQualifiedName, tExists.AssemblyQualifiedName);
                    WriteLine(error);

                    throw new ApplicationException(error);
                }

                WriteLine("Hash: 0x{0:x8} {1}", hash, t.FullName);

                _htLookupType[hash] = t;
                _htLookupHash[t] = hash;
            }
            catch
            {
                WriteLine("AddType failed: {0}", t.FullName);
                _htLookupHash[t] = INVALID_HASH_ENTRY;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private Type LookupType(uint hash)
        {
            Type t = _htLookupType[hash] as Type;

            return t;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private uint LookupHash(
            Type t,
            bool throwOnTypeNotFound = false)
        {
            object o = _htLookupHash[t];

            if (o == null)
            {
                if (AddType(t))
                {
                    o = _htLookupHash[t];

                    return (uint)o;
                }
            }
            else
            {
                return (uint)o;
            }

            if (o == null
                && throwOnTypeNotFound)
            {
                throw new System.Runtime.Serialization.SerializationException();
            }

            return 0xFFFFFFFF;
        }

        private uint LookupAssemblyHash(Assembly assm)
        {
            AssemblyName name = assm.GetName();

            return LookupAssemblyHash(name.Name, name.Version);
        }

        private uint LookupAssemblyHash(string assemblyName, Version version)
        {
            uint hash;

            hash = ComputeHashForName(assemblyName, 0);
            hash = ComputeHashForUShort((ushort)version.Major, hash);
            hash = ComputeHashForUShort((ushort)version.Minor, hash);
            hash = ComputeHashForUShort((ushort)version.Build, hash);
            hash = ComputeHashForUShort((ushort)version.Revision, hash);

            return hash;
        }

        private uint ComputeHashForType(
            Type t,
            uint hash)
        {
            hash = ComputeHashForName(t.FullName, hash);

            while (t != null)
            {
                foreach (FieldInfo f in t.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if ((f.Attributes & FieldAttributes.NotSerialized) != 0)
                    {
                        continue;
                    }

                    hash = ComputeHashForField(f, hash);
                }

                t = t.BaseType;
            }

            return hash;
        }

        private uint ComputeHashForField(FieldInfo f, uint hash)
        {
            Type t = f.FieldType;
            TypeDescriptor td;
            TypeDescriptorBasic tdSub;
            string name = f.Name;

            //
            // Special case for core types that have different representation on client and server.
            //
            if (f.DeclaringType == typeof(MemberInfo))
            {
                if (name == "m_cachedData")
                {
                    return hash;
                }
            }

            if (f.DeclaringType == typeof(DateTime))
            {
                if (name == "ticks")
                {
                    name = "_ticks";
                }

                if (name == "dateData" ||
                    name == "_dateData")
                {
                    name = "_ticks";
                    t = typeof(ulong);
                }
            }

            td = new TypeDescriptor(t);

            if (td._base.IsArray)
            {
                int depth = td._arrayDepth;
                while (depth-- > 0)
                {
                    hash = ComputeHashForType(ElementType.PELEMENT_TYPE_SZARRAY, hash);
                }

                tdSub = td._arrayElement;
            }
            else
            {
                tdSub = td._base;
            }

            hash = ComputeHashForType(tdSub._et, hash);

            switch (tdSub._et)
            {
                case ElementType.PELEMENT_TYPE_CLASS:
                case ElementType.PELEMENT_TYPE_VALUETYPE:
                    hash = ComputeHashForName(tdSub.Type.FullName, hash);
                    break;
            }

            hash = ComputeHashForName(name, hash);

            return hash;
        }

        private uint ComputeHashForType(ElementType et, uint hash)
        {
            uint hashPost = CrcHelper.ComputeCrc((byte)et, hash);

            return hashPost;
        }

        private uint ComputeHashForName(string s, uint hash)
        {
            uint hashPost = CrcHelper.ComputeCrc(Encoding.UTF8.GetBytes(s), hash);

            return hashPost;
        }

        private uint ComputeHashForUShort(ushort val, uint hash)
        {
            hash = CrcHelper.ComputeCrc((byte)(val >> 0), hash);
            hash = CrcHelper.ComputeCrc((byte)(val >> 8), hash);

            return hash;
        }

        [Conditional("TRACE_SERIALIZATION")]
        public static void WriteLine(string fmt, params object[] lst)
        {
            Console.WriteLine(fmt, lst);
        }

        internal int BitsAvailable()
        {
            return _stream.BitsAvailable;
        }

        internal void WriteBits(uint val, int bits)
        {
            _stream.WriteBits(val, bits);
        }

        internal uint ReadBits(int bits)
        {
            return _stream.ReadBits(bits);
        }

        internal void WriteBitsLong(ulong val, int bits)
        {
            int extra = bits - 32;

            if (extra > 0)
            {
                _stream.WriteBits((uint)(val >> 32), extra);

                bits = 32;
            }

            _stream.WriteBits((uint)val, bits);
        }

        internal ulong ReadBitsLong(int bits)
        {
            ulong val;

            int extra = bits - 32;

            if (extra > 0)
            {
                val = (ulong)_stream.ReadBits(extra) << 32;
                bits = 32;
            }
            else
            {
                val = 0;
            }

            val |= _stream.ReadBits(bits);

            return val;
        }

        internal void WriteArray(byte[] data, int pos, int len)
        {
            _stream.WriteArray(data, pos, len);
        }

        internal void ReadArray(byte[] data, int pos, int len)
        {
            _stream.ReadArray(data, pos, len);
        }

        internal void WriteCompressedUnsigned(uint val)
        {
            if (val == 0xFFFFFFFF)
            {
                _stream.WriteBits(0xFF, 8);
            }
            else if (val < 0x80)
            {
                _stream.WriteBits(val, 8);
            }
            else
            {
                if (val < 0x3F00)
                {
                    _stream.WriteBits(0x8000 | val, 16);
                }
                else if (val < 0x3F000000)
                {
                    _stream.WriteBits(0xC0000000 | val, 32);
                }
                else
                {
                    throw new ArgumentException("Max value is 0x3F000000");
                }
            }
        }

        internal uint ReadCompressedUnsigned()
        {
            uint val = _stream.ReadBits(8);

            if (val == 0xFF)
            {
                return 0xFFFFFFFF;
            }

            switch (val & 0xC0)
            {
                case 0x00: break;
                case 0x40: break;
                case 0x80: val = ((val & ~0xC0U) << 8) | _stream.ReadBits(8); break;
                case 0xC0: val = ((val & ~0xC0U) << 24) | _stream.ReadBits(24); break;
            }

            return val;
        }

        internal void WriteType(Type t)
        {
            _stream.WriteBits(LookupHash(t, true), 32);
        }

        internal Type ReadType()
        {
            uint hash = _stream.ReadBits(32);
            Type t = LookupType(hash);

            if (t == null)
            {
                throw new System.Runtime.Serialization.SerializationException(string.Format("Cannot find type for hash {0:X8}", hash));
            }

            return t;
        }

#if DEBUG

        public byte[] DebugSerialize(
            object graph,
            string pathPrefix)
        {
            return DebugSerialize(
                graph,
                null,
                pathPrefix);
        }

        public byte[] DebugSerialize(
            object graph,
            Type type,
            string pathPrefix)
        {
            byte[] buf;

            try
            {
                buf = Serialize(type, graph);
            }
            catch
            {
                DateTime dt = DateTime.Now;

                TraceDumpOriginalObject(
                    graph,
                    pathPrefix,
                    dt);

                throw;
            }

            if (buf != null)
            {
                object o2;

                try
                {
                    o2 = Deserialize(type, buf);
                }
                catch
                {
                    DateTime dt = DateTime.Now;

                    TraceDumpOriginalObject(
                        graph,
                        pathPrefix,
                        dt);

                    TraceDumpSerializedObject(
                        buf,
                        pathPrefix,
                        dt);

                    throw;
                }
            }

            return buf;
        }

        void TraceDumpOriginalObject(
            object graph,
            string pathPrefix,
            DateTime dt)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                bf.Serialize(stream, graph);

                SaveToFile(stream.ToArray(), pathPrefix, "orig", dt);
            }
            catch
            {
            }
        }

        void TraceDumpSerializedObject(
            byte[] buffer,
            string pathPrefix,
            DateTime dt)
        {
            try
            {
                SaveToFile(buffer, pathPrefix, "ser", dt);
            }
            catch
            {
            }
        }

        void SaveToFile(
            byte[] buffer,
            string pathPrefix,
            string pathSuffix,
            DateTime dt)
        {
            string file = string.Format("{0}_{2:yyyyMMdd}_{2:Hmmss}.{1}", pathPrefix, pathSuffix, dt);

            using (FileStream s = new(
                file,
                FileMode.Create,
                FileAccess.Write))
            {
                s.Write(buffer, 0, buffer.Length);
                s.Close();
            }
        }

#endif //DEBUG

    }
}