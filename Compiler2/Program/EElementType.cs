using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler2.Program
{
    enum EElementType
    {
        End =   0x00,
        Void =  0x01,
        Bool =  0x02,
        Char =  0x03, //unicode character
        SignedByte = 0x04,
        Byte =  0x05,
        Int16 = 0x06,
        UInt16 = 0x07,
        Int32 = 0x08,
        UInt32 = 0x09,
        Int64 = 0x0a,
        UInt64 = 0x0b,
        Float32 = 0x0c,
        Float64 = 0x0d,
        String = 0x0e,
        Pointer = 0x0f, //unmanaged, next byte is type
        Reference = 0x10, //managed pointer, next byte is type
        ValueType = 0x11, //modifier, followed by typedef/typref token
        Class = 0x12, //modifier, follwed by typedef/typref token
        Var = 0x13, //?
        Array = 0x14, //modifier
        GenericInst = 0x15,
        TypedRef = 0x16,
        IntPtr = 0x18,
        UIntPtr = 0x19,
        FnPtr = 0x1b,
        Object = 0x1c,
        SzArray = 0x1d,
        MVar= 0x1e,
        CMod_Required = 0x1f,
        CMod_Optional = 0x20,
        Internal = 0x21,
        Modifier = 0x40,
        Sentinel = 0x41,
        Pinned = 0x45,
        Type = 0x50,
        Box = 0x51,
        Field = 0x53,
        Property = 0x54,
        Enum = 0x55
    }

    static class EElementTypeExtenstions
    {
        public static bool IsCustomMod(this EElementType type)
        {
            return type == EElementType.CMod_Optional || type == EElementType.CMod_Required;
        }
    }
}
