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