using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet.Utils
{
    public static class BitsConverter
    {
        public static List<bool> ToBits(this BigInteger bigIntegerValue)
        {
            return bigIntegerValue.BigIntToBytes(32).BytesToBits(255);
        }

        public static List<bool> ToBits(this string stringValue)
        {
            var msgByte = Encoding.UTF8.GetBytes(stringValue);
            var bits = msgByte.Select(x =>
            {
                var list = new List<byte> { x };
                var byteTobit = list.BytesToBits();
                byteTobit.Reverse();
                return byteTobit;
            }).SelectMany(x => x).ToList();
            return bits;
        }

        public static List<bool> ToBits(this UInt64 uint64Value)
        {
            // sizes is length / 8
            return ((BigInteger)uint64Value).BigIntToBytes(8).BytesToBits(64);
        }

        public static List<bool> ToBits(this UInt32 uint32Value)
        {
            return ((BigInteger)uint32Value).BigIntToBytes(4).BytesToBits(32);
        }

        public static List<bool> ToBits(this bool boolValue)
        {
            return new List<bool> { boolValue };
        }
    }
}
