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

        // the modulus. called `p` in most of our code.
        public BigInteger p = BigInteger.Parse("40000000000000000000000000000000224698fc094cf91b992d30ed00000001", NumberStyles.HexNumber);

        public BigInteger q = BigInteger.Parse("40000000000000000000000000000000224698fc0994a8dd8c46eb2100000001", NumberStyles.HexNumber);

        public BigInteger S { get; set; }

        public PrivateKey(string base58)
        {
            List<byte> decode = base58.Decode().ToList();

            BigInteger x = new BigInteger(decode.ToArray());

            BigInteger y = mod(x, p);


            var checksum = new List<byte>(decode);
            checksum.RemoveRange(0, decode.Count - 5);

            var originalBytes = base58.Decode().ToList();
            originalBytes.RemoveRange(originalBytes.Count - 4, 4);

            Debug.WriteLine("original bytes 0 " + originalBytes[0]);

            if (originalBytes[0] != VersionByte)
            {
                throw new Exception($"fromBase58Check: input version byte ${VersionByte} does not match encoded version byte ${originalBytes[0]}");
            }

            originalBytes.RemoveAt(0);

            this.S = BinableBigIntegerFromBytes(originalBytes.ToArray());
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

        private BigInteger mod(BigInteger input, BigInteger mod)
        {
            BigInteger x = input % mod;
            if (x < 0) return x + mod;
            return x;
        }

        // https://github.com/o1-labs/o1js-bindings/blob/5e5befc8579393dadb96be1917642f860624ed07/lib/provable-bigint.ts#L62
        private BigInteger BinableBigIntegerFromBytes(byte[] bytes)
        {
            int end = bytes.Length;
            BigInteger x = BigInteger.Zero;
            int bitPosition = 0;
            // first byte is verif byte escape it
            for (int i = 1; i < end; i++)
            {
                x += new BigInteger(bytes[i]) << bitPosition;
                bitPosition += 8;
            }
            return x;
        }

    }
}
