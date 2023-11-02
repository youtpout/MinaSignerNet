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


            var packedInput = PackToFields(input);
            var inputBits = packedInput.SelectMany(x => BytesToBits(x.BigIntToBytes(32).ToList()).Take(255)).ToList();
            var inputBytes = BitsToBytes(inputBits);
            // let inputBits = packedInput.Select(x=> x.BytesToBigInt()).flat();
            return new byte[0];
        }

        public static List<bool> BytesToBits(List<byte> bytes)
        {
            return bytes
              .SelectMany((b) =>
              {
                  var bits = new Boolean[8];
                  for (var i = 0; i < 8; i++)
                  {
                      bits[i] = (b & 1) == 1;
                      b >>= 1;
                  }
                  return bits;
              }).ToList();
        }

        public static List<byte> BitsToBytes(List<bool> bits)
        {
            var bytes = new List<byte>();
            int size = bits.Count;
            int index = 0;
            while (index < size)
            {
                var byteBits = bits.Skip(index).Take(8);
                index += 8;
                byte b = 0x0;
                for (var i = 0; i < byteBits.Count(); i++)
                {
                    if (!byteBits.ElementAt(i)) continue;
                    b |= (byte)(1 << i);
                }
                bytes.Add(b);
            }
            return bytes;
        }


        public static List<BigInteger> PackToFields(HashInput hashInput)
        {
            if (hashInput.Packed.Count == 0) return hashInput.Fields;
            List<BigInteger> packedBits = new List<BigInteger>();
            var currentPackedField = BigInteger.Zero;
            int currentSize = 0;

            foreach (var item in hashInput.Packed)
            {
                var size = item.Item2;
                var field = item.Item1;
                currentSize += size;
                if (currentSize < 255)
                {
                    currentPackedField = currentPackedField * (BigInteger.One << size) + field;
                }
                else
                {
                    packedBits.Add(currentPackedField);
                    currentSize = size;
                    currentPackedField = field;
                }
            }
            packedBits.Add(currentPackedField);
            hashInput.Fields.AddRange(packedBits);
            return hashInput.Fields;
        }


        public override string ToString()
        {
            return base.ToString();
        }

    }
}
