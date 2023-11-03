using Blake2Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MinaSignerNet
{
    public class Signature
    {
        public BigInteger R { get; set; }
        public BigInteger S { get; set; }


        /// <summary>
        /// Sign a message from a private key
        /// </summary>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="message">message to sign</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>Signature</returns>
        public static Signature Create(string privateKey, BigInteger message, int networkId = Constants.NetworkIdMainnet)
        {
            var pKey = new PrivateKey(privateKey);
            var kPrime = DeriveNonce(message, pKey, networkId);
            var group = Group.FromNonce(kPrime);
            var k = group.Y.BigIntToBytes(32).BytesToBits()[0] ? FiniteField.Negate(kPrime, Constants.P) : kPrime;
            return new Signature() { R = group.X };
        }


        public static BigInteger DeriveNonce(BigInteger message, PrivateKey privateKey, int networkId)
        {
            var input = new HashInput();
            var group = Group.FromPrivateKey(privateKey);

            input.Fields.Add(message);
            input.Fields.Add(group.X);
            input.Fields.Add(group.Y);
            input.Fields.Add(privateKey.S);

            List<BigInteger> dataPacked = new List<BigInteger>();
            input.Packed.Add(new Tuple<BigInteger, int>(new BigInteger(networkId), 8));


            var packedInput = input.PackToFields();
            var inputBits = packedInput.SelectMany(x => x.BigIntToBytes(32).BytesToBits().Take(255)).ToList();
            var inputBytes = inputBits.BitsToBytes();

            Blake2BConfig config = new Blake2BConfig() { OutputSizeInBytes = 32 };
            var outputBytes = Blake2B.ComputeHash(inputBytes.ToArray(), config);
            // drop the top two bits to convert into a scalar field element
            // (creates negligible bias because q = 2^254 + eps, eps << q)
            outputBytes[outputBytes.Count() - 1] &= 0x3f;

            return new BigInteger(outputBytes);
        }



        public override string ToString()
        {
            return base.ToString();
        }

    }
}
