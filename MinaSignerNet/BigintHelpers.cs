using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace MinaSignerNet
{
    public static class BigintHelpers
    {
        public static BigInteger BytesToBigInt(byte[] bytes)
        {
            var x = BigInteger.Zero;
            int bitPosition = 0;
            foreach (var item in bytes)
            {
                x += new BigInteger(item) << bitPosition;
                bitPosition += 8;
            }
            return x;
        }

        public static BigInteger parseHexString(string input)
        {
            // Parse the bytes explicitly, Bigint endianness is wrong
            var inputBytes = new byte[32];
            for (var j = 0; j < 32; j++)
            {
                inputBytes[j] = Convert.ToByte(input[2 * j] + input[2 * j + 1]);
            }
            return BytesToBigInt(inputBytes);
        }

        /**
         * Transforms bigint to little-endian array of bytes (numbers between 0 and 255) of a given length.
         * Throws an error if the bigint doesn't fit in the given number of bytes.
         */
        public static byte[] BigIntToBytes(BigInteger x, int length)
        {
            if (x < BigInteger.Zero)
            {
                throw new Exception($"bigIntToBytes: negative numbers are not supported, got {x}");
            }
            var bytes = new byte[length];
            for (var i = 0; i < length; i++, x >>= 8)
            {
                bytes[i] = ((byte)(x & 255));
            }
            if (x > BigInteger.Zero)
            {
                throw new Exception($"bigIntToBytes: input does not fit in {length} bytes");
            }
            return bytes;
        }

    }
}
