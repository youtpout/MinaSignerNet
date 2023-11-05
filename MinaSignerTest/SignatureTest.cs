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
            string s = "5699199801648180374790470641706672153292517155570336605316501766706790369496";
            string r = "4571424820326310551923378628151304413301975448848148859180947955631174949047";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(message, privKey, Network.Testnet);

            Assert.Equal(BigInteger.Parse(s), signature.S);
            Assert.Equal(BigInteger.Parse(r), signature.R);
            output.WriteLine("signature " + signature.ToString());
            Assert.Equal(signatureBase58, signature.ToString());

            var isGood = Signature.Verify(signature, message, pubKey, Network.Testnet);
            Assert.True(isGood);
        }

        [Fact]
        public void SignatureIncorrect()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            BigInteger message = BigInteger.Parse("123456");
            string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "5699199801648180374790470641706672153292517155570336605316501766706790369496";
            string r = "4571424820326310551923378628151304413301975448848148859180947955631174949047";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(message, privKey, Network.Testnet);
            signature.R += 1;
            var isGood = Signature.Verify(signature, message, pubKey, Network.Testnet);
            Assert.False(isGood);
        }

        [Fact]
        public void SignString()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            string message = "Hello";
            string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "5699199801648180374790470641706672153292517155570336605316501766706790369496";
            string r = "4571424820326310551923378628151304413301975448848148859180947955631174949047";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(message, privKey, Network.Testnet);

            //Assert.Equal(BigInteger.Parse(s), signature.S);
            //Assert.Equal(BigInteger.Parse(r), signature.R);
            //output.WriteLine("signature " + signature.ToString());
            //Assert.Equal(signatureBase58, signature.ToString());

            //var isGood = Signature.Verify(signature, message, pubKey, Network.Testnet);
            //Assert.True(isGood);
        }

    }
}