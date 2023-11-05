using Blake2Core;
using MinaSignerNet;
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

        public const byte VersionNumber = 1;
        public const byte VersionByte = 154;
        public BigInteger R { get; set; }
        public BigInteger S { get; set; }


        /// <summary>
        /// Sign a message from a private key
        /// </summary>        
        /// <param name="message">message to sign</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>Signature</returns>
        public static Signature Sign(BigInteger message, string privateKey, Network networkId = Network.Mainnet)
        {
            var pKey = new PrivateKey(privateKey);
            var kPrime = DeriveNonce(message, pKey, networkId);
            var groupPKey = Group.FromPrivateKey(pKey);
            var groupKPrime = Group.FromNonce(kPrime);
            var r = groupKPrime.X;
            var k = groupKPrime.Y.IsEven ? kPrime : FiniteField.Negate(kPrime, Constants.Q);
            var concat = new List<BigInteger> { message, groupPKey.X, groupPKey.Y, r };
            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            var e = PoseidonHash.HashWithPrefix(prefix, concat);
            var s = FiniteField.Add(k, FiniteField.Mul(e, pKey.S, Constants.Q), Constants.Q);

            return new Signature() { R = r, S = s };
        }


        /// <summary>
        /// Verifies a signature created by Sign method, returns `true` if (and only if) the signature is valid. 
        /// </summary>
        /// <param name="signature">signature to check</param>
        /// <param name="message">original message</param>
        /// <param name="publicKey">public key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>True if correct</returns>
        public static bool Verify(Signature signature, BigInteger message, string publicKey, Network networkId = Network.Mainnet)
        {
            var pubKey = new PublicKey(publicKey);
            //var kPrime = DeriveNonce(message, pKey, networkId);
            //var groupPKey = Group.FromPrivateKey(pKey);
            //var groupKPrime = Group.FromNonce(kPrime);
            //var r = groupKPrime.X;
            //var k = groupKPrime.Y.IsEven ? kPrime : FiniteField.Negate(kPrime, Constants.Q);
            //var concat = new List<BigInteger> { message, groupPKey.X, groupPKey.Y, r };
            //var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            //var e = PoseidonHash.HashWithPrefix(prefix, concat);
            //var s = FiniteField.Add(k, FiniteField.Mul(e, pKey.S, Constants.Q), Constants.Q);

            return false;
        }


        public static BigInteger DeriveNonce(BigInteger message, PrivateKey privateKey, Network networkId)
        {
            var input = new HashInput();
            var group = Group.FromPrivateKey(privateKey);

            input.Fields.Add(message);
            input.Fields.Add(group.X);
            input.Fields.Add(group.Y);
            input.Fields.Add(privateKey.S);

            List<BigInteger> dataPacked = new List<BigInteger>();
            input.Packed.Add(new Tuple<BigInteger, int>(new BigInteger((int)networkId), 8));


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

        public static BigInteger HashMessage(HashInput input, PublicKey publicKey, BigInteger r, Network networkId)
        {
            //var input = HashInput.append(message, { fields: [x, y, r] });
            //let prefix =
            //  networkId === 'mainnet'
            //    ? prefixes.signatureMainnet
            //    : prefixes.signatureTestnet;
            //return hashWithPrefix(prefix, packToFields(input));
            return new BigInteger();
        }

        public override string ToString()
        {
            var bytesR = R.BigIntToBytes(32);
            var bytesS = S.BigIntToBytes(32);
            List<byte> bytes = new List<byte>(bytesR.Concat(bytesS));
            // add version number in first place
            bytes.Insert(0, VersionNumber);

            return bytes.ToArray().ToBase58Check(VersionByte);
        }

    }
}
