using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public static class EllipticCurve
    {
        /// <summary>
        /// Calculate projective scale
        /// </summary>
        /// <param name="g">Projective from calculate</param>
        /// <param name="x">Private key S</param>
        /// <param name="p">Modulus P</param>
        public static GroupProjective ProjectiveScale(GroupProjective g, BigInteger x, BigInteger p)
        {
            var h = Constants.ProjectiveZero;

            while (x > BigInteger.Zero)
            {
                if ((x & 1) == 1)
                {
                    h = ProjectiveAdd(h, g, p);

                }
                g = ProjectiveDouble(g, p);
                x >>= 1;
            }
            return h;
        }

        public static GroupProjective ProjectiveAdd(GroupProjective g, GroupProjective h, BigInteger p)
        {
            if (g.Z == BigInteger.Zero)
                return h;
            if (h.Z == BigInteger.Zero)
                return g;
            BigInteger X1 = g.X, Y1 = g.Y, Z1 = g.Z, X2 = h.X, Y2 = h.Y, Z2 = h.Z;
            // http://www.hyperelliptic.org/EFD/g1p/auto-shortw-jacobian-0.html#addition-add-2007-bl
            // Z1Z1 = Z1^2
            var Z1Z1 = FiniteField.Mod(Z1 * Z1, p);
            // Z2Z2 = Z2^2
            var Z2Z2 = FiniteField.Mod(Z2 * Z2, p);
            // U1 = X1*Z2Z2
            var U1 = FiniteField.Mod(X1 * Z2Z2, p);
            // U2 = X2*Z1Z1
            var U2 = FiniteField.Mod(X2 * Z1Z1, p);
            // S1 = Y1*Z2*Z2Z2
            var S1 = FiniteField.Mod(Y1 * Z2 * Z2Z2, p);
            // S2 = Y2*Z1*Z1Z1
            var S2 = FiniteField.Mod(Y2 * Z1 * Z1Z1, p);
            // H = U2-U1
            var H = FiniteField.Mod(U2 - U1, p);
            // H = 0 <==> x1 = X1/Z1^2 = X2/Z2^2 = x2 <==> degenerate case (Z3 would become 0)
            if (H == BigInteger.Zero)
            {
                // if S1 = S2 <==> y1 = y2, the points are equal, so we double instead
                if (S1 == S2)
                    return ProjectiveDouble(g, p);
                // if S1 = -S2, the points are inverse, so return zero
                if (FiniteField.Mod(S1 + S2, p) == BigInteger.Zero)
                    return Constants.ProjectiveZero;
                throw new Exception("ProjectiveAdd: invalid point");
            }
            // I = (2*H)^2
            var I = FiniteField.Mod((H * H) << 2, p);
            // J = H*I
            var J = FiniteField.Mod(H * I, p);
            // r = 2*(S2-S1)
            var r = 2 * (S2 - S1);
            // V = U1*I
            var V = FiniteField.Mod(U1 * I, p);
            // X3 = r^2-J-2*V
            var X3 = FiniteField.Mod(r * r - J - 2 * V, p);
            // Y3 = r*(V-X3)-2*S1*J
            var Y3 = FiniteField.Mod(r * (V - X3) - 2 * S1 * J, p);
            // Z3 = ((Z1+Z2)^2-Z1Z1-Z2Z2)*H
            var Z3 = FiniteField.Mod(((Z1 + Z2) * (Z1 + Z2) - Z1Z1 - Z2Z2) * H, p);
            return new GroupProjective() { X = X3, Y = Y3, Z = Z3 };
        }

        public static GroupProjective ProjectiveDouble(GroupProjective g, BigInteger p)
        {
            if (g.Z == BigInteger.Zero)
                return g;
            BigInteger X1 = g.X, Y1 = g.Y, Z1 = g.Z;
            if (Y1 == BigInteger.Zero)
                throw new Exception("ProjectiveDouble: unhandled case");
            // http://www.hyperelliptic.org/EFD/g1p/auto-shortw-jacobian-0.html#doubling-dbl-2009-l
            // !!! formula depends on a === 0 in the curve equation y^2 = x^3 + ax + b !!!
            // A = X1^2
            var A = FiniteField.Mod(X1 * X1, p);
            // B = Y1^2
            var B = FiniteField.Mod(Y1 * Y1, p);
            // C = B^2
            var C = FiniteField.Mod(B * B, p);
            // D = 2*((X1+B)^2-A-C)
            var D = FiniteField.Mod(2 * ((X1 + B) * (X1 + B) - A - C), p);
            // E = 3*A
            var E = 3 * A;
            // F = E^2
            var F = FiniteField.Mod(E * E, p);
            // X3 = F-2*D
            var X3 = FiniteField.Mod(F - 2 * D, p);
            // Y3 = E*(D-X3)-8*C
            var Y3 = FiniteField.Mod(E * (D - X3) - 8 * C, p);
            // Z3 = 2*Y1*Z1
            var Z3 = FiniteField.Mod(2 * Y1 * Z1, p);
            return new GroupProjective() { X = X3, Y = Y3, Z = Z3 };
        }

        public static GroupAffine ProjectiveToAffine(GroupProjective g, BigInteger p)
        {
            var z = g.Z;
            if (z == BigInteger.Zero)
            {
                // infinity
                return new GroupAffine() { X = BigInteger.Zero, Y = BigInteger.Zero, Infinity = true };
            }
            else if (z == BigInteger.One)
            {
                // already normalized affine form
                return new GroupAffine() { X = g.X, Y = g.Y, Infinity = false };
            }
            else
            {
                var zinv = FiniteField.Inverse(z, p); // we checked for z === 0, so inverse exists
                var zinv_squared = FiniteField.Mod(zinv * zinv, p);
                // x/z^2
                var x = FiniteField.Mod(g.X * zinv_squared, p);
                // y/z^3
                var y = FiniteField.Mod(g.Y * zinv * zinv_squared, p);
                return new GroupAffine() { X = x, Y = y, Infinity = false };
            }
        }

    }
}
