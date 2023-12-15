using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public enum Network
    {
        Mainnet = 1,
        Testnet = 0
    }

    public static class Constants
    {
        public const string SignatureMainnet = "MinaSignatureMainnet";
        public const string SignatureTestnet = "CodaSignature*******";

        public static List<bool> LegacyTokenId = new List<bool>() { };

        static Constants()
        {
            // const legacyTokenId = [true, ...Array<boolean>(63).fill(false)];
            LegacyTokenId.Add(true);
            for (int i = 0; i < 63; i++)
            {
                LegacyTokenId.Add(false);
            }
        }

        public static GroupProjective PallasGeneratorProjective = new GroupProjective()
        {
            X = BigInteger.One,
            Y = BigInteger.Parse("12418654782883325593414442427049395787963493412651469444558597405572177144507"),
            Z = BigInteger.One
        };

        public static GroupProjective ProjectiveZero = new GroupProjective()
        {
            X = BigInteger.One,
            Y = BigInteger.One,
            Z = BigInteger.Zero
        };



        // the modulus. called `p` in most of our code.
        public static BigInteger P = BigInteger.Parse("40000000000000000000000000000000224698fc094cf91b992d30ed00000001", NumberStyles.HexNumber);

        public static BigInteger Q = BigInteger.Parse("40000000000000000000000000000000224698fc0994a8dd8c46eb2100000001", NumberStyles.HexNumber);

        public static BigInteger ScalarShift = BigInteger.Parse("28948022309329048855892746252171976963271935850878634640049049255563201871872");

        public static BigInteger OneHalf = BigInteger.Parse("14474011154664524427946373126085988481681528240970823689839871374196681474049");
        // this is `t`, where p = 2^32 * t + 1
        public static BigInteger pMinusOneOddFactor = BigInteger.Parse("40000000000000000000000000000000224698fc094cf91b992d30ed", NumberStyles.HexNumber);
        public static BigInteger qMinusOneOddFactor = BigInteger.Parse("40000000000000000000000000000000224698fc0994a8dd8c46eb21", NumberStyles.HexNumber);
        // primitive roots of unity, computed as (5^t mod p). this works because 5 generates the multiplicative group mod p
        public static BigInteger twoadicRootFp = BigInteger.Parse("2bce74deac30ebda362120830561f81aea322bf2b7bb7584bdad6fabd87ea32f", NumberStyles.HexNumber);
        public static BigInteger twoadicRootFq = BigInteger.Parse("2de6a9b8746d3f589e5c4dfd492ae26e9bb97ea3c106f049a70e2c1102b6d05f", NumberStyles.HexNumber);
    }
}
