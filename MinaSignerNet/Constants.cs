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

        public static BigInteger ScalarShift =  BigInteger.Parse("28948022309329048855892746252171976963271935850878634640049049255563201871872");

        public static BigInteger OneHalf = BigInteger.Parse("14474011154664524427946373126085988481681528240970823689839871374196681474049");

    }
}
