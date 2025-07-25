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
            BigInteger message = BigInteger.Parse("12");
            var list = new List<BigInteger> { message };

            BigInteger hash = PoseidonHash.Hash(list);
            var expected = BigInteger.Parse("20307190475163560179843878304233687113040243867319358507811895775846718326775");

            Assert.Equal(expected, hash);
        }



    }
}