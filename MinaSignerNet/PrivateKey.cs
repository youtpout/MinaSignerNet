using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class PrivateKey
    {
        /*  let versionBytes = {
    "tokenIdKey": 28,
    "receiptChainHash": 12,
    "ledgerHash": 5,
    "epochSeed": 13,
    "stateHash": 16,
    "publicKey": 203,
    "userCommandMemo": 20,
    "privateKey": 90,
    "signature": 154,
    "transactionHash": 29,
    "signedCommandV1": 19
  };*/

        public const byte VersionByte = 90;
        public const byte VersionNumber = 1;

        public BigInteger S { get; set; }

        public PrivateKey(string base58)
        {
            List<byte> decode = base58.FromBase58Check(VersionByte).ToList();

            //  first byte is for verification
            decode.RemoveAt(0);

            this.S = decode.ToArray().BytesToBigInt();
        }

        public PublicKey GetPublicKey()
        {
            var group = Group.FromPrivateKey(this);
            var pubKey = new PublicKey(group.X, !group.Y.IsEven);
            return pubKey;

        }


        public override string ToString()
        {
            var bytesX = S.BigIntToBytes(32);
            List<byte> bytes = new List<byte>(bytesX);
            // add version number in first place
            bytes.Insert(0, VersionNumber);

            return bytes.ToArray().ToBase58Check(VersionByte);
        }
    }
}
