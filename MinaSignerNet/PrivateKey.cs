using System;
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

            var originalBytes = new List<byte>(decode);
            originalBytes.RemoveRange(decode.Count - 5, 4);

            Debug.WriteLine("original bytes 0 " + originalBytes[0]);

            if (originalBytes[0] != VersionByte)
            {
                throw new Exception($"fromBase58Check: input version byte ${VersionByte} does not match encoded version byte ${originalBytes[0]}");
            }

            decode.RemoveRange(0, 4);

            var result = new List<byte>(base58.Decode());
            result.RemoveAt(0);

            string hexBytes = ByteArrayToString(result.ToArray()).Substring(0, 66);

            List<byte> rebyte = base58.Decode().Skip(2).ToList();
            rebyte.Reverse();

            //Array.Reverse(res, 0, res.Length);

            this.S = new BigInteger(rebyte.ToArray()); ;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }
}
