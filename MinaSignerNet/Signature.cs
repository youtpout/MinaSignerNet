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
            DeriveNonce(message, pKey, networkId);
            return new Signature();
        }


        public static byte[] DeriveNonce(BigInteger message, PrivateKey privateKey, int networkId)
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
            // let inputBits = packedInput.Select(x=> x.BytesToBigInt()).flat();
            return new byte[0];
        }



        public override string ToString()
        {
            return base.ToString();
        }

    }
}
