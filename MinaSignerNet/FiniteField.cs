﻿using System;
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


        public static BigInteger Mod(BigInteger x, BigInteger p)
        {
            x = x % p;
            if (x < 0)
                return x + p;
            return x;
        }

        public static BigInteger Negate(BigInteger x, BigInteger p)
        {
            return x == BigInteger.Zero ? BigInteger.Zero : p - x;
        }

        public static BigInteger Inverse(BigInteger a, BigInteger p)
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

        public static BigInteger Power(BigInteger a, BigInteger n, BigInteger p)
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

        public static BigInteger Dot(List<BigInteger> x, List<BigInteger> y, BigInteger p)
        {
            var z = BigInteger.Zero;
            var n = x.Count;
            for (var i = 0; i < n; i++)
            {
                z += x[i] * y[i];
            }
            return Mod(z, p);
        }
        public static BigInteger Add(BigInteger x, BigInteger y, BigInteger p)
        {
            return Mod(x + y, p);
        }

        public static BigInteger Mul(BigInteger x, BigInteger y, BigInteger p)
        {
            return Mod(x * y, p);
        }

        public static BigInteger Sub(BigInteger x, BigInteger y, BigInteger p)
        {
            return Mod(x - y, p);
        }


    }

}
