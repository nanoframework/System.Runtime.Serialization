//
// Copyright (c) .NET Foundation and Contributors
// Portions Copyright (c) Microsoft Corporation.  All rights reserved.
// See LICENSE file in the project root for full license information.
//

namespace nanoFramework.Serialization.Helper
{
    // KEEP IN SYNC WITH CLR_CorElementType!!
    internal enum ElementType : byte
    {
        //
        // This is based on CorElementType, but adds a few types for support of boxing/unboxing.
        //
        PELEMENT_TYPE_END = 0x0,
        PELEMENT_TYPE_VOID = 0x1,
        PELEMENT_TYPE_BOOLEAN = 0x2,
        PELEMENT_TYPE_CHAR = 0x3,
        PELEMENT_TYPE_I1 = 0x4,
        PELEMENT_TYPE_U1 = 0x5,
        PELEMENT_TYPE_I2 = 0x6,
        PELEMENT_TYPE_U2 = 0x7,
        PELEMENT_TYPE_I4 = 0x8,
        PELEMENT_TYPE_U4 = 0x9,
        PELEMENT_TYPE_I8 = 0xa,
        PELEMENT_TYPE_U8 = 0xb,
        PELEMENT_TYPE_R4 = 0xc,
        PELEMENT_TYPE_R8 = 0xd,
        PELEMENT_TYPE_STRING = 0xe,

        // every type above PTR will be simple type
        // PTR <type>
        PELEMENT_TYPE_PTR = 0xf,
        // BYREF <type>
        PELEMENT_TYPE_BYREF = 0x10,

        // Please use ELEMENT_TYPE_VALUETYPE. ELEMENT_TYPE_VALUECLASS is deprecated.
        // VALUETYPE <class Token>
        PELEMENT_TYPE_VALUETYPE = 0x11,
        // CLASS <class Token>
        PELEMENT_TYPE_CLASS = 0x12,

        // MDARRAY <type> <rank> <bcount> <bound1> ... <lbcount> <lb1> ...
        PELEMENT_TYPE_ARRAY = 0x14,

        // This is a simple type.
        PELEMENT_TYPE_TYPEDBYREF = 0x16,

        // native integer size
        PELEMENT_TYPE_I = 0x18,
        // native unsigned integer size
        PELEMENT_TYPE_U = 0x19,
        // FNPTR <complete sig for the function including calling convention>
        PELEMENT_TYPE_FNPTR = 0x1B,
        // Shortcut for System.Object
        PELEMENT_TYPE_OBJECT = 0x1C,
        // Shortcut for single dimension zero lower bound array
        PELEMENT_TYPE_SZARRAY = 0x1D,
        // SZARRAY <type>

        // This is only for binding
        // required C modifier : E_T_CMOD_REQD <mdTypeRef/mdTypeDef>
        PELEMENT_TYPE_CMOD_REQD = 0x1F,
        // optional C modifier : E_T_CMOD_OPT <mdTypeRef/mdTypeDef>
        PELEMENT_TYPE_CMOD_OPT = 0x20,

        // This is for signatures generated internally (which will not be persisted in any way).
        // INTERNAL <typehandle>
        PELEMENT_TYPE_INTERNAL = 0x21,

        // Note that this is the max of base type excluding modifiers
        // first invalid element type
        PELEMENT_TYPE_MAX = 0x22,

        //
        // End of overlap with CorElementType.
        //
    };
}