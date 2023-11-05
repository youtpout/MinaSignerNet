using MinaSignerNet;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;

namespace MinaSignerTest
{
    public class PublicKeyTest
    {
        private readonly ITestOutputHelper output;

        public PublicKeyTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GenerateFromBase58()
        {
            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";
            BigInteger decoded = BigInteger.Parse("63110719032097344795227873669858709628934331129599862827491186742779629074");

            PublicKey key = PublicKey.FromBase58(pubKey);

            output.WriteLine("decoding result : " + key.X.ToString());
            Assert.Equal(decoded, key.X);

            Assert.Equal(pubKey, key.ToString());
        }


    }
}