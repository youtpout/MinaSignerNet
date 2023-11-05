using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    /// <summary>
    /// GENERAL FINITE FIELD ALGORITHMS
    /// https://github.com/o1-labs/o1js-bindings/blob/2a60a9bd3b9f6936dc8178af478ce14732c04056/crypto/finite_field.ts#L24C1-L24C35
    /// </summary>
    public static class FiniteField
    {


        public static BigInteger Mod(this BigInteger x, BigInteger p)
        {
            x = x % p;
            if (x < 0)
                return x + p;
            return x;
        }

        public static BigInteger Negate(this BigInteger x, BigInteger p)
        {
            return x == BigInteger.Zero ? BigInteger.Zero : p - x;
        }

        public static BigInteger Inverse(this BigInteger a, BigInteger p)
        {
            a = Mod(a, p);
            if (a == BigInteger.Zero) return BigInteger.Zero;
            var b = p;
            var x = BigInteger.Zero;
            var y = BigInteger.One;
            var u = BigInteger.One;
            var v = BigInteger.Zero;
            while (a != BigInteger.Zero)
            {
                var q = b / a;
                var r = Mod(b, a);
                var m = x - u * q;
                var n = y - v * q;
                b = a;
                a = r;
                x = u;
                y = v;
                u = m;
                v = n;
            }
            if (b != BigInteger.One) return BigInteger.Zero;
            return Mod(x, p);
        }

        public static BigInteger Power(this BigInteger a, BigInteger n, BigInteger p)
        {
            a = Mod(a, p);
            var x = BigInteger.One;
            for (; n > BigInteger.Zero; n >>= 1)
            {
                if ((n & 1) == 1) x = Mod(x * a, p);
                a = Mod(a * a, p);
            }
            return x;
        }

        public static BigInteger Dot(this List<BigInteger> x, List<BigInteger> y, BigInteger p)
        {
            var z = BigInteger.Zero;
            var n = x.Count;
            for (var i = 0; i < n; i++)
            {
                z += x[i] * y[i];
            }
            return Mod(z, p);
        }
        public static BigInteger Add(this BigInteger x, BigInteger y, BigInteger p)
        {
            return Mod(x + y, p);
        }

        public static BigInteger Mul(this BigInteger x, BigInteger y, BigInteger p)
        {
            return Mod(x * y, p);
        }

        public static BigInteger Sub(this BigInteger x, BigInteger y, BigInteger p)
        {
            return Mod(x - y, p);
        }

        public static BigInteger sqrt(this BigInteger n, BigInteger p, BigInteger Q, BigInteger c)
        {
            // https://en.wikipedia.org/wiki/Tonelli-Shanks_algorithm#The_algorithm
            // variable naming is the same as in that link ^
            // Q is what we call `t` elsewhere - the odd factor in p - 1
            // c is a known primitive root of unity
            if (n == BigInteger.Zero)
                return BigInteger.Zero;
            var M = 32;
            var t = Power(n, (Q - 1) >> 1, p); // n^(Q - 1)/2
            var R = Mod(t * n, p); // n^((Q - 1)/2 + 1) = n^((Q + 1)/2)
            t = Mod(t * R, p); // n^((Q - 1)/2 + (Q + 1)/2) = n^Q
            while (true)
            {
                if (t == BigInteger.One)
                    return R;
                // use repeated squaring to find the least i, 0 < i < M, such that t^(2^i) = 1
                var i = 0;
                var s = t;
                while (s != BigInteger.One)
                {
                    s = Mod(s * s, p);
                    i = i + 1;
                }
                if (i == M)
                    return BigInteger.Zero; // no solution
                var b = Power(c, BigInteger.One << (M - i - 1), p); // c^(2^(M-i-1))
                M = i;
                c = Mod(b * b, p);
                t = Mod(t * c, p);
                R = Mod(R * b, p);
            }
        }


    }

}
