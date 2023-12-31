﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MinaSignerNet
{
    // source : https://gist.github.com/micli/c242edd2a81a8f0d9f7953842bcc24f1
    public static class Base58
    {
        public static readonly char[] _alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();
        private static int[] _indexes = new int[128];

        static Base58()
        {
            for (int i = 0; i < _indexes.Length; i++)
            {
                _indexes[i] = -1;
            }
            for (int i = 0; i < _alphabet.Length; i++)
            {
                _indexes[_alphabet[i]] = i;
            }
        }

        public static string EncodeFromString(this string input)
        {
            byte[] datas = Encoding.UTF8.GetBytes(input);
            return Encode(datas);
        }

        public static string DecodeFromBytes(this byte[] input)
        {
            string datas = Encoding.UTF8.GetString(input);
            byte[] decoded = Decode(datas);
            return Encoding.UTF8.GetString(decoded);
        }

        public static string DecodeToString(this string input)
        {
            byte[] decoded = Decode(input);
            return Encoding.UTF8.GetString(decoded);
        }

        public static byte[] FromBase58Check(this string base58, byte versionByte)
        {
            // throws on invalid character
            var bytes = Decode(base58);
            // check checksum
            var checksum = new List<byte>(bytes);
            checksum.RemoveRange(0, checksum.Count - 4);

            var originalBytes = bytes.ToList();
            originalBytes.RemoveRange(originalBytes.Count - 4, 4);

            var actualChecksum = ComputeChecksum(originalBytes.ToArray());
            if (!ArrayEqual(checksum.ToArray(), actualChecksum))
                throw new Exception("FromBase58Check: invalid checksum");
            // check version byte
            if (originalBytes[0] != versionByte)
                throw new Exception($"FromBase58Check: input version byte ${versionByte} does not match encoded version byte ${originalBytes[0]}");
            // return result
            originalBytes.RemoveAt(0);
            return originalBytes.ToArray();
        }

        public static bool ArrayEqual(byte[] firstArray, byte[] secondArray)
        {
            if (firstArray.Length != secondArray.Length)
                return false;
            for (int i = 0; i < firstArray.Length; i++)
            {
                if (firstArray[i] != secondArray[i])
                    return false;
            }
            return true;
        }

        public static string ToBase58Check(this byte[] input, byte versionByte)
        {
            var withVersion = new List<byte>(input);
            withVersion.Insert(0, versionByte);
            var arr = withVersion.ToArray();
            var checksum = ComputeChecksum(arr);
            var withChecksum = withVersion.Concat(checksum);
            return Encode(withChecksum.ToArray());
        }

        public static byte[] ComputeChecksum(this byte[] input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashValue1 = sha256.ComputeHash(input);
                byte[] hashValue2 = sha256.ComputeHash(hashValue1);
                var checksum = hashValue2.ToList().Take(4).ToArray();
                return checksum;
            }
        }

        public static string Encode(this byte[] input)
        {
            if (0 == input.Length)
            {
                return String.Empty;
            }
            input = CopyOfRange(input, 0, input.Length);
            // Count leading zeroes.
            int zeroCount = 0;
            while (zeroCount < input.Length && input[zeroCount] == 0)
            {
                zeroCount++;
            }
            // The actual encoding.
            byte[] temp = new byte[input.Length * 2];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input.Length)
            {
                byte mod = DivMod58(input, startAt);
                if (input[startAt] == 0)
                {
                    startAt++;
                }
                temp[--j] = (byte)_alphabet[mod];
            }

            // Strip extra '1' if there are some after decoding.
            while (j < temp.Length && temp[j] == _alphabet[0])
            {
                ++j;
            }
            // Add as many leading '1' as there were leading zeros.
            while (--zeroCount >= 0)
            {
                temp[--j] = (byte)_alphabet[0];
            }

            byte[] output = CopyOfRange(temp, j, temp.Length);
            try
            {
                return Encoding.ASCII.GetString(output);
            }
            catch (DecoderFallbackException e)
            {
                Console.WriteLine(e.ToString());
                return String.Empty;
            }
        }
        public static byte[] Decode(this string input)
        {
            if (0 == input.Length)
            {
                return new byte[0];
            }
            byte[] input58 = new byte[input.Length];
            // Transform the String to a base58 byte sequence
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                int digit58 = -1;
                if (c >= 0 && c < 128)
                {
                    digit58 = _indexes[c];
                }
                if (digit58 < 0)
                {
                    throw new ArgumentException("Illegal character " + c + " at " + i);
                }

                input58[i] = (byte)digit58;
            }
            // Count leading zeroes
            int zeroCount = 0;
            while (zeroCount < input58.Length && input58[zeroCount] == 0)
            {
                zeroCount++;
            }
            // The encoding
            byte[] temp = new byte[input.Length];
            int j = temp.Length;

            int startAt = zeroCount;
            while (startAt < input58.Length)
            {
                byte mod = DivMod256(input58, startAt);
                if (input58[startAt] == 0)
                {
                    ++startAt;
                }
                temp[--j] = mod;
            }
            // Do no add extra leading zeroes, move j to first non null byte.
            while (j < temp.Length && temp[j] == 0)
            {
                j++;
            }
            return CopyOfRange(temp, j - zeroCount, temp.Length);
        }

        static byte DivMod58(byte[] number, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number.Length; i++)
            {
                int digit256 = (int)number[i] & 0xFF;
                int temp = remainder * 256 + digit256;

                number[i] = (byte)(temp / 58);

                remainder = temp % 58;
            }

            return (byte)remainder;
        }
        static byte DivMod256(byte[] number58, int startAt)
        {
            int remainder = 0;
            for (int i = startAt; i < number58.Length; i++)
            {
                int digit58 = (int)number58[i] & 0xFF;
                int temp = remainder * 58 + digit58;

                number58[i] = (byte)(temp / 256);

                remainder = temp % 256;
            }

            return (byte)remainder;
        }
        static byte[] CopyOfRange(byte[] source, int from, int to)
        {
            byte[] range = new byte[to - from];
            for (int i = 0; i < to - from; i++)
            {
                range[i] = source[from + i];
            }
            return range;
        }

    }
}
