using System;
using System.Collections.Generic;
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
