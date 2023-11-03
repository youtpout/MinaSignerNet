using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class Group
    {


        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }


        // https://github.com/o1-labs/o1js-bindings/blob/2a60a9bd3b9f6936dc8178af478ce14732c04056/crypto/elliptic_curve.ts#L216
        public static Group FromPrivateKey(PrivateKey key)
        {

            var projectiveScale = EllipticCurve.ProjectiveScale(Constants.PallasGeneratorProjective, key.S, Constants.P);
            var projToAffine = EllipticCurve.ProjectiveToAffine(projectiveScale, Constants.P);

            return projToAffine;
        }

        public static Group FromNonce(BigInteger nonce)
        {

            var projectiveScale = EllipticCurve.ProjectiveScale(Constants.PallasGeneratorProjective, nonce, Constants.P);
            var projToAffine = EllipticCurve.ProjectiveToAffine(projectiveScale, Constants.P);

            return projToAffine;
        }

    }
}
