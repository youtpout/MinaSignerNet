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


        public BigInteger S { get; set; }

        public PrivateKey(string base58)
        {
            List<byte> decode = base58.Decode().ToList();

            var checksum = new List<byte>(decode);
            checksum.RemoveRange(0, decode.Count - 5);

            var originalBytes = base58.Decode().ToList();
            originalBytes.RemoveRange(originalBytes.Count - 4, 4);

            //Debug.WriteLine("original bytes 0 " + originalBytes[0]);

            if (originalBytes[0] != VersionByte)
            {
                throw new Exception($"fromBase58Check: input version byte ${VersionByte} does not match encoded version byte ${originalBytes[0]}");
            }

            // 2 first bytes is for verification
            originalBytes.RemoveRange(0, 2);

            this.S = originalBytes.ToArray().BytesToBigInt();
        }

        public PublicKey GetPublicKey()
        {
            var group = Group.FromPrivateKey(this);
            var pubKey = new PublicKey() { IsOdd = (group.Y & 1) == 1, X = group.X };
            return pubKey;

        }
    }
}
