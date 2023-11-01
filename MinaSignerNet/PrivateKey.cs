using System;
using System.Collections.Generic;
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

        public const int VersionBytes = 90;

        public BigInteger S { get; set; }

        public PrivateKey(string base58)
        {
            byte[] decode = base58.Decode();

            var b = new BigInteger(VersionBytes);
            var version = b.ToByteArray();
            this.S = new BigInteger(version.Concat(decode).ToArray());
        }

    }
}
