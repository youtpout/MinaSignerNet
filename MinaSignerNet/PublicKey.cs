using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class PublicKey
    {
        public const byte VersionNumber = 1;
        public const byte VersionByte = 203;
        public bool IsOdd { get; set; }
        public BigInteger X { get; set; }

        public PublicKey(BigInteger x, bool isOdd)
        {
            this.X = x;
            this.IsOdd = isOdd;
        }

        public PublicKey(string base58)
        {
            var decoded = base58.FromBase58Check(VersionByte);
            // we ignore the 2 first number who were version number and the last who is odd indicator
            X = decoded.Skip(2).Take(32).BytesToBigInt();
            IsOdd = decoded.Last() == 0b1;
        }


        public override string ToString()
        {
            var bytesX = X.BigIntToBytes(32);
            List<byte> bytes = new List<byte>(bytesX);
            // add version number (twice) in first place
            bytes.Insert(0, VersionNumber);
            bytes.Insert(0, VersionNumber);
            // odd in last place
            if (IsOdd)
            {
                bytes.Add(1);
            }
            else
            {
                bytes.Add(0);
            }


            return bytes.ToArray().ToBase58Check(VersionByte);
        }
    }
}
