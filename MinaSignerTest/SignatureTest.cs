using MinaSignerNet;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;

namespace MinaSignerTest
{
    public class SignatureTest
    {
        private readonly ITestOutputHelper output;

        public SignatureTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void SignMessage()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            BigInteger message = BigInteger.Parse("123456");
            string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";

            Signature signature = Signature.Create(privKey, message, Network.Testnet);

            output.WriteLine("signature " + signature.ToString());
            Assert.Equal(signatureBase58, signature.ToString());
        }



    }
}