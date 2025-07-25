//using MinaSignerNet.Utils;
//using MinaSignerNet;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Numerics;
//using System.Text;
//using System.Linq;

//namespace MinaSignerMinimal
//{

//    public struct MSignature
//    {
//        public const byte VersionNumber = 1;
//        public const byte VersionByte = 154;
//        public BigInteger R { get; set; }
//        public BigInteger S { get; set; }
//    }

//    public struct MPublicKey
//    {
//        public const byte VersionNumber = 1;
//        public const byte VersionByte = 203;
//        public bool IsOdd { get; set; }
//        public BigInteger X { get; set; }
//    }

//    public struct MGroup
//    {

//        public BigInteger X { get; set; }
//        public BigInteger Y { get; set; }
//    }


//    public struct MGroupProjective
//    {
//        public BigInteger X { get; set; }
//        public BigInteger Y { get; set; }
//        public BigInteger Z { get; set; }
//    }

//    public static class MConstants
//    {
//        public const string SignatureMainnet = "MinaSignatureMainnet";
//        public const string SignatureTestnet = "CodaSignature*******";

//        public static List<bool> LegacyTokenId = new List<bool>() { };

//        static MConstants()
//        {
//            // const legacyTokenId = [true, ...Array<boolean>(63).fill(false)];
//            LegacyTokenId.Add(true);
//            for (int i = 0; i < 63; i++)
//            {
//                LegacyTokenId.Add(false);
//            }
//        }

//        public static MGroupProjective PallasGeneratorProjective = new MGroupProjective()
//        {
//            X = BigInteger.One,
//            Y = BigInteger.Parse("12418654782883325593414442427049395787963493412651469444558597405572177144507"),
//            Z = BigInteger.One
//        };

//        public static MGroupProjective ProjectiveZero = new MGroupProjective()
//        {
//            X = BigInteger.One,
//            Y = BigInteger.One,
//            Z = BigInteger.Zero
//        };



//        // the modulus. called `p` in most of our code.
//        public static BigInteger P = BigInteger.Parse("40000000000000000000000000000000224698fc094cf91b992d30ed00000001", NumberStyles.HexNumber);

//        public static BigInteger Q = BigInteger.Parse("40000000000000000000000000000000224698fc0994a8dd8c46eb2100000001", NumberStyles.HexNumber);

//        public static BigInteger ScalarShift = BigInteger.Parse("28948022309329048855892746252171976963271935850878634640049049255563201871872");

//        public static BigInteger OneHalf = BigInteger.Parse("14474011154664524427946373126085988481681528240970823689839871374196681474049");
//        // this is `t`, where p = 2^32 * t + 1
//        public static BigInteger pMinusOneOddFactor = BigInteger.Parse("40000000000000000000000000000000224698fc094cf91b992d30ed", NumberStyles.HexNumber);
//        public static BigInteger qMinusOneOddFactor = BigInteger.Parse("40000000000000000000000000000000224698fc0994a8dd8c46eb21", NumberStyles.HexNumber);
//        // primitive roots of unity, computed as (5^t mod p). this works because 5 generates the multiplicative group mod p
//        public static BigInteger twoadicRootFp = BigInteger.Parse("2bce74deac30ebda362120830561f81aea322bf2b7bb7584bdad6fabd87ea32f", NumberStyles.HexNumber);
//        public static BigInteger twoadicRootFq = BigInteger.Parse("2de6a9b8746d3f589e5c4dfd492ae26e9bb97ea3c106f049a70e2c1102b6d05f", NumberStyles.HexNumber);
//    }

//    public static class Verifier
//    {

//        public static bool Verify(MSignature signature, List<BigInteger> messages, MPublicKey publicKey, bool mainnet=true)
//        {
//            var groupPubKey = FromPublickKey(publicKey);
//            var concat = new List<BigInteger>(messages);
//            concat.Add(groupPubKey.X);
//            concat.Add(groupPubKey.Y);
//            concat.Add(signature.R);

//            var prefix = mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
//            var e = PoseidonHash.HashWithPrefix(prefix, concat);
//            var scale = EllipticCurve.ProjectiveScale(Constants.PallasGeneratorProjective, signature.S, Constants.P);
//            var groupProj = new GroupProjective(groupPubKey);
//            var scalePubKey = EllipticCurve.ProjectiveScale(groupProj, e, Constants.P);
//            var R = EllipticCurve.ProjectiveSub(scale, scalePubKey, Constants.P);
//            try
//            {
//                // if `R` is infinity, Group.fromProjective throws an error, so `verify` returns false
//                Group grpFinal = R.ToGroup();
//                return grpFinal.Y.IsEven && grpFinal.X == signature.R;
//            }
//            catch
//            {
//                return false;
//            }
//        }

//        public static MGroup FromPublickKey(MPublicKey pubKey)
//        {
//            var x = pubKey.X;
//            var ySquared = x.Mul(x, MConstants.P).Mul(x, MConstants.P).Add(5, MConstants.P);
//            var someY = ySquared.sqrt(MConstants.P, MConstants.pMinusOneOddFactor, MConstants.twoadicRootFp);
//            var isTheRightY = pubKey.IsOdd == (!someY.IsEven);
//            var y = isTheRightY.BoolToBigInteger()
//            .Mul(someY, MConstants.P)
//            .Add((!isTheRightY).BoolToBigInteger().Mul(someY.Negate(MConstants.P), MConstants.P), MConstants.P);
//            return new MGroup() { X = x, Y = y };
//        }
//    }

//    public static class FiniteField
//    {


//        public static BigInteger Mod(this BigInteger x, BigInteger p)
//        {
//            x = x % p;
//            if (x < 0)
//                return x + p;
//            return x;
//        }

//        public static BigInteger Negate(this BigInteger x, BigInteger p)
//        {
//            return x == BigInteger.Zero ? BigInteger.Zero : p - x;
//        }

//        public static BigInteger Inverse(this BigInteger a, BigInteger p)
//        {
//            a = Mod(a, p);
//            if (a == BigInteger.Zero) return BigInteger.Zero;
//            var b = p;
//            var x = BigInteger.Zero;
//            var y = BigInteger.One;
//            var u = BigInteger.One;
//            var v = BigInteger.Zero;
//            while (a != BigInteger.Zero)
//            {
//                var q = b / a;
//                var r = Mod(b, a);
//                var m = x - u * q;
//                var n = y - v * q;
//                b = a;
//                a = r;
//                x = u;
//                y = v;
//                u = m;
//                v = n;
//            }
//            if (b != BigInteger.One) return BigInteger.Zero;
//            return Mod(x, p);
//        }

//        public static BigInteger Power(this BigInteger a, BigInteger n, BigInteger p)
//        {
//            a = Mod(a, p);
//            var x = BigInteger.One;
//            for (; n > BigInteger.Zero; n >>= 1)
//            {
//                if ((n & 1) == 1) x = Mod(x * a, p);
//                a = Mod(a * a, p);
//            }
//            return x;
//        }

//        public static BigInteger Dot(this List<BigInteger> x, List<BigInteger> y, BigInteger p)
//        {
//            var z = BigInteger.Zero;
//            var n = x.Count;
//            for (var i = 0; i < n; i++)
//            {
//                z += x[i] * y[i];
//            }
//            return Mod(z, p);
//        }
//        public static BigInteger Add(this BigInteger x, BigInteger y, BigInteger p)
//        {
//            return Mod(x + y, p);
//        }

//        public static BigInteger Mul(this BigInteger x, BigInteger y, BigInteger p)
//        {
//            return Mod(x * y, p);
//        }

//        public static BigInteger Sub(this BigInteger x, BigInteger y, BigInteger p)
//        {
//            return Mod(x - y, p);
//        }

//        public static BigInteger sqrt(this BigInteger n, BigInteger p, BigInteger Q, BigInteger c)
//        {
//            // https://en.wikipedia.org/wiki/Tonelli-Shanks_algorithm#The_algorithm
//            // variable naming is the same as in that link ^
//            // Q is what we call `t` elsewhere - the odd factor in p - 1
//            // c is a known primitive root of unity
//            if (n == BigInteger.Zero)
//                return BigInteger.Zero;
//            var M = 32;
//            var t = Power(n, (Q - 1) >> 1, p); // n^(Q - 1)/2
//            var R = Mod(t * n, p); // n^((Q - 1)/2 + 1) = n^((Q + 1)/2)
//            t = Mod(t * R, p); // n^((Q - 1)/2 + (Q + 1)/2) = n^Q
//            while (true)
//            {
//                if (t == BigInteger.One)
//                    return R;
//                // use repeated squaring to find the least i, 0 < i < M, such that t^(2^i) = 1
//                var i = 0;
//                var s = t;
//                while (s != BigInteger.One)
//                {
//                    s = Mod(s * s, p);
//                    i = i + 1;
//                }
//                if (i == M)
//                    return BigInteger.Zero; // no solution
//                var b = Power(c, BigInteger.One << (M - i - 1), p); // c^(2^(M-i-1))
//                M = i;
//                c = Mod(b * b, p);
//                t = Mod(t * c, p);
//                R = Mod(R * b, p);
//            }
//        }
//    }

//    public static class PoseidonHash
//    {
//        public static BigInteger HashWithPrefix(string prefix, List<BigInteger> input)
//        {
//            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
//            var prefixBigInteger = PrefixToBigInteger(prefix);
//            var list = new List<BigInteger> { prefixBigInteger };
//            // salt prefix
//            var init = PoseidonUpdate(initialState, list, PoseidonConstant.PoseidonConfigKimchiFp);
//            return PoseidonUpdate(init, input, PoseidonConstant.PoseidonConfigKimchiFp).First();
//        }

//        public static BigInteger HashWithPrefixLegacy(string prefix, List<BigInteger> input)
//        {
//            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
//            var prefixBigInteger = PrefixToBigInteger(prefix);
//            var list = new List<BigInteger> { prefixBigInteger };
//            // salt prefix
//            var init = PoseidonUpdate(initialState, list, PoseidonConstant.PoseidonConfigLegacyFp);
//            return PoseidonUpdate(init, input, PoseidonConstant.PoseidonConfigLegacyFp).First();
//        }

//        public static BigInteger HashMessageLegacy(HashInputLegacy messages, PrivateKey privateKey, BigInteger r, Network networkId)
//        {
//            var input = new HashInputLegacy();
//            input.Add(messages);
//            var group = Group.FromPrivateKey(privateKey);
//            input.Fields.Add(group.X);
//            input.Fields.Add(group.Y);
//            input.Fields.Add(r);

//            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
//            return HashWithPrefixLegacy(prefix, input.GetFieldsLegacy());
//        }

//        public static BigInteger HashMessageLegacy(HashInputLegacy messages, Group groupPublicKey, BigInteger r, Network networkId)
//        {
//            var input = new HashInputLegacy();
//            input.Add(messages);
//            input.Fields.Add(groupPublicKey.X);
//            input.Fields.Add(groupPublicKey.Y);
//            input.Fields.Add(r);

//            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
//            return HashWithPrefixLegacy(prefix, input.GetFieldsLegacy());
//        }

//        public static BigInteger Hash(List<BigInteger> input)
//        {
//            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
//            return PoseidonUpdate(initialState, input, PoseidonConstant.PoseidonConfigKimchiFp).First();
//        }

//        public static BigInteger PrefixToBigInteger(string prefix)
//        {
//            int fieldSize = 32;
//            if (prefix.Length >= fieldSize)
//                throw new Exception("prefix too long");

//            var stringBytes = Encoding.UTF8.GetBytes(prefix);

//            var array = new byte[fieldSize];
//            stringBytes.CopyTo(array, 0);

//            return array.BytesToBigInt();
//        }

//        public static List<BigInteger> PoseidonUpdate(List<BigInteger> state, List<BigInteger> input, PoseidonConfig config)
//        {
//            if (input.Count == 0)
//            {
//                Permutation(state, config);
//                return state;
//            }
//            // pad input with zeros so its length is a multiple of the rate
//            decimal n = Math.Ceiling((decimal)input.Count / config.Rate) * config.Rate;
//            var array = new BigInteger[(int)n];
//            input.CopyTo(array, 0);
//            // for every block of length `rate`, add block to the first `rate` elements of the state, and apply the permutation
//            for (var blockIndex = 0; blockIndex < n; blockIndex += config.Rate)
//            {
//                for (var i = 0; i < config.Rate; i++)
//                {
//                    state[i] = FiniteField.Add(state[i], array[blockIndex + i], Constants.P);
//                }
//                Permutation(state, config);
//            }

//            return state;
//        }


//        public static void Permutation(List<BigInteger> state, PoseidonConfig config)
//        {
//            // special case: initial round constant
//            var offset = 0;
//            if (config.HasInitialRoundConstant)
//            {
//                for (var i = 0; i < config.StateSize; i++)
//                {
//                    state[i] = FiniteField.Add(state[i], config.RoundConstants[0][i], Constants.P);
//                }
//                offset = 1;
//            }
//            for (var round = 0; round < config.FullRounds; round++)
//            {
//                // raise to a power
//                for (var i = 0; i < config.StateSize; i++)
//                {
//                    state[i] = FiniteField.Power(state[i], config.Power, Constants.P);
//                }
//                var oldState = new List<BigInteger>(state);
//                for (var i = 0; i < config.StateSize; i++)
//                {
//                    // multiply by mds matrix
//                    state[i] = FiniteField.Dot(config.Mds[i], oldState, Constants.P);
//                    // add round constants
//                    state[i] = FiniteField.Add(state[i], config.RoundConstants[round + offset][i], Constants.P);
//                }
//            }
//        }


//    }
//}
