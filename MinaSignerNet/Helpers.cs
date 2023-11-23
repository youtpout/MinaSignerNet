using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace MinaSignerNet
{
    public static class Helpers
    {
        public static BigInteger BytesToBigInt(this IEnumerable<byte> bytes)
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

        public static BigInteger ParseHexString(this string input)
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
        public static byte[] BigIntToBytes(this BigInteger x, int length)
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

        public static string ByteArrayToString(this byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] StringToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static BigInteger BoolToBigInteger(this bool value)
        {
            return value ? BigInteger.One : BigInteger.Zero;
        }

        /// <summary>
        /// Convert bytes array to bool array
        /// </summary>
        /// <param name="bytes">input array</param>
        /// <param name="maxSize">max size of output array</param>
        /// <returns>the array in bool format</returns>
        public static List<bool> BytesToBits(this IEnumerable<byte> bytes, int maxSize = int.MaxValue)
        {
            return bytes
              .SelectMany((b) =>
              {
                  var bits = new Boolean[8];
                  for (var i = 0; i < 8; i++)
                  {
                      bits[i] = (b & 1) == 1;
                      b >>= 1;
                  }
                  return bits;
              }).Take(maxSize).ToList();
        }

        public static List<byte> BitsToBytes(this IEnumerable<bool> bits)
        {
            var bytes = new List<byte>();
            int size = bits.Count();
            int index = 0;
            while (index < size)
            {
                var byteBits = bits.Skip(index).Take(8);
                index += 8;
                byte b = 0x0;
                for (var i = 0; i < byteBits.Count(); i++)
                {
                    if (!byteBits.ElementAt(i)) continue;
                    b |= (byte)(1 << i);
                }
                bytes.Add(b);
            }
            return bytes;
        }

        public static List<BigInteger> PackToFields(this HashInput hashInput)
        {
            if (hashInput.Packed.Count == 0) return hashInput.Fields;
            List<BigInteger> packedBits = new List<BigInteger>();
            var currentPackedField = BigInteger.Zero;
            int currentSize = 0;

            foreach (var item in hashInput.Packed)
            {
                var size = item.Item2;
                var field = item.Item1;
                currentSize += size;
                if (currentSize < 255)
                {
                    currentPackedField = currentPackedField * (BigInteger.One << size) + field;
                }
                else
                {
                    packedBits.Add(currentPackedField);
                    currentSize = size;
                    currentPackedField = field;
                }
            }
            packedBits.Add(currentPackedField);
            hashInput.Fields.AddRange(packedBits);
            return hashInput.Fields;
        }

        public static BigInteger Shift(this BigInteger s)
        {
            return FiniteField.Add(FiniteField.Add(s, s, Constants.P), Constants.ScalarShift, Constants.P);
        }

        public static BigInteger Unshift(this BigInteger s)
        {
            var sub = FiniteField.Sub(s, Constants.ScalarShift, Constants.P);
            var mul = FiniteField.Mul(sub, Constants.OneHalf, Constants.P);
            return mul;
        }

    }
}
