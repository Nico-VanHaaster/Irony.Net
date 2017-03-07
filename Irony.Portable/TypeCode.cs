using System;

namespace System
{
#if _PCL_

    public static class TypeCodeExtensions
    {
        /// <summary>
        /// returns the tytpe
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        public static Type ToType(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return typeof(bool);
                case TypeCode.Byte: return typeof(byte);
                case TypeCode.Char: return typeof(char);
                case TypeCode.DateTime: return typeof(DateTime);
                case TypeCode.Decimal: return typeof(decimal);
                case TypeCode.Double: return typeof(double);
                case TypeCode.Int16: return typeof(short);
                case TypeCode.Int32: return typeof(int);
                case TypeCode.Int64: return typeof(long);
                case TypeCode.Object: return typeof(object);
                case TypeCode.SByte: return typeof(sbyte);
                case TypeCode.Single: return typeof(float);
                case TypeCode.String: return typeof(string);
                case TypeCode.UInt16: return typeof(ushort);
                case TypeCode.UInt32: return typeof(uint);
                case TypeCode.UInt64: return typeof(ulong);

                case TypeCode.Empty:
                case TypeCode.DBNull:
                default:
                    return typeof(string);
            }
        }
    }

    //
    // Summary:
    //     Specifies the type of an object.
    public enum TypeCode
    {
        //
        // Summary:
        //     A null reference.
        Empty = 0,
        //
        // Summary:
        //     A general type representing any reference or value type not explicitly represented
        //     by another TypeCode.
        Object = 1,
        //
        // Summary:
        //     A database null (column) value.
        DBNull = 2,
        //
        // Summary:
        //     A simple type representing Boolean values of true or false.
        Boolean = 3,
        //
        // Summary:
        //     An integral type representing unsigned 16-bit integers with values between 0
        //     and 65535. The set of possible values for the System.TypeCode.Char type corresponds
        //     to the Unicode character set.
        Char = 4,
        //
        // Summary:
        //     An integral type representing signed 8-bit integers with values between -128
        //     and 127.
        SByte = 5,
        //
        // Summary:
        //     An integral type representing unsigned 8-bit integers with values between 0 and
        //     255.
        Byte = 6,
        //
        // Summary:
        //     An integral type representing signed 16-bit integers with values between -32768
        //     and 32767.
        Int16 = 7,
        //
        // Summary:
        //     An integral type representing unsigned 16-bit integers with values between 0
        //     and 65535.
        UInt16 = 8,
        //
        // Summary:
        //     An integral type representing signed 32-bit integers with values between -2147483648
        //     and 2147483647.
        Int32 = 9,
        //
        // Summary:
        //     An integral type representing unsigned 32-bit integers with values between 0
        //     and 4294967295.
        UInt32 = 10,
        //
        // Summary:
        //     An integral type representing signed 64-bit integers with values between -9223372036854775808
        //     and 9223372036854775807.
        Int64 = 11,
        //
        // Summary:
        //     An integral type representing unsigned 64-bit integers with values between 0
        //     and 18446744073709551615.
        UInt64 = 12,
        //
        // Summary:
        //     A floating point type representing values ranging from approximately 1.5 x 10
        //     -45 to 3.4 x 10 38 with a precision of 7 digits.
        Single = 13,
        //
        // Summary:
        //     A floating point type representing values ranging from approximately 5.0 x 10
        //     -324 to 1.7 x 10 308 with a precision of 15-16 digits.
        Double = 14,
        //
        // Summary:
        //     A simple type representing values ranging from 1.0 x 10 -28 to approximately
        //     7.9 x 10 28 with 28-29 significant digits.
        Decimal = 15,
        //
        // Summary:
        //     A type representing a date and time value.
        DateTime = 16,
        //
        // Summary:
        //     A sealed class type representing Unicode character strings.
        String = 18
    }
#endif
}
