using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MinaSignerPoseidon;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MinaSignerTest
{
    public class PoseidonTest
    {
        private readonly ITestOutputHelper output;

        public PoseidonTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ConvertMds()
        {
            output.WriteLine("const MAT_INTERNAL_DIAG_M_1: &'static [FpPallas] = &[");
            foreach (var value in PoseidonConstant.PoseidonConfigKimchiFp.Mds.SelectMany(x => x))
            {
                string hex = BigIntToRustHex(value);
                output.WriteLine($"    fp_from_hex!(\"{hex}\"),");
            }
            output.WriteLine("];");
        }

        [Fact]
        public void RoundConstants()
        {
            output.WriteLine("const ROUND_CONSTANTS: &'static [&'static [FpKimchi]] = &[");
            foreach (var row in PoseidonConstant.PoseidonConfigKimchiFp.RoundConstants)
            {
                output.WriteLine("    &[");
                for (int i = 0; i < row.Count; i++)
                {
                    string hex = BigIntToRustHex(row[i]);
                    output.WriteLine($"fp_from_hex!(\"{hex}\")");
                    if (i < row.Count - 1)
                        output.WriteLine(", ");
                }
                output.WriteLine("],");
            }
            output.WriteLine("];");
        }

        static string BigIntToRustHex(BigInteger value)
        {
            if (value.Sign < 0)
                throw new Exception("Negative values are not supported for Fp elements.");

            // Convert to big-endian byte array
            byte[] bytes = value.ToByteArray(isUnsigned: true, isBigEndian: true);

            // Pad to 32 bytes (256 bits)
            if (bytes.Length > 32)
                throw new Exception("Value too large for 256-bit field element.");

            byte[] padded = new byte[32];
            Array.Copy(bytes, 0, padded, 32 - bytes.Length, bytes.Length);

            return BitConverter.ToString(padded).Replace("-", "").ToLowerInvariant();
        }

        [Fact]
        public void Hash()
        {

            BigInteger message = BigInteger.Parse("12");
            var list = new List<BigInteger> { message };

            BigInteger hash = PoseidonHash.Hash(list);
            output.WriteLine("hash " + hash);
            var expected = BigInteger.Parse("20307190475163560179843878304233687113040243867319358507811895775846718326775");

            Assert.Equal(expected, hash);
        }


        [Fact]
        public void HashTwo()
        {
            BigInteger message = BigInteger.Parse("3412");
            BigInteger message2 = BigInteger.Parse("548748548");
            var list = new List<BigInteger> { message, message2 };

            BigInteger hash = PoseidonHash.Hash(list);
            var expected = BigInteger.Parse("24245350037390325723675562428846509781869515058976947458013661211417354108422");

            Assert.Equal(expected, hash);
        }

        [Fact]
        public void HashNegative()
        {
            List<BigInteger> list = new List<BigInteger>
{
    BigInteger.Parse("-35545"),
    BigInteger.Zero,
    BigInteger.Parse("-7878454"),
    BigInteger.Parse("45524"),
    BigInteger.MinusOne
};

            BigInteger hash = PoseidonHash.Hash(list);
            var expected = BigInteger.Parse("17944732201997716732580423582703197695777318095974149487644452711464169895343");

            Assert.Equal(expected, hash);
        }


        [Fact]
        public void HashMultiple()
        {

            List<BigInteger> list = new List<BigInteger>
{
    BigInteger.Parse("7263514276861361464633452875109919113182485937660059416421780822303488364810"),
    BigInteger.Zero,
    BigInteger.Parse("14045753958617862754670070034440311287432747428158303518301078328357441472472"),
    BigInteger.One,
    BigInteger.Zero,
    BigInteger.Zero,
    new BigInteger(1000000000),
    BigInteger.Parse("23155254961521224263414982804952607540765699032359833230215492926554005931079"),
    BigInteger.Zero,
    new BigInteger(8000000000),
    new BigInteger(1000000000),
    BigInteger.Zero,
    BigInteger.One
};


            BigInteger hash = PoseidonHash.Hash(list);
            var expected = BigInteger.Parse("9477952037298579560464731632706228830146195233473478290570101327698566553181");

            Assert.Equal(expected, hash);
        }

    }
}