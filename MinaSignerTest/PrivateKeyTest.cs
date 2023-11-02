using MinaSignerNet;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;

namespace MinaSignerTest
{
    public class PrivateKeyTest
    {
        private readonly ITestOutputHelper output;

        public PrivateKeyTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GenerateFromBase58()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            string s = "6394367615778924328473768566388294210789075330077747486508293976718144629785";
            PrivateKey pKey = new PrivateKey(privKey);
            var grp = Group.FromPrivateKey(pKey);
            output.WriteLine("decoding result : " + pKey.S.ToString());
            Assert.Equal(s, pKey.S.ToString());
            var pKeyStr = pKey.ToString();
            Assert.Equal(privKey, pKeyStr);

            var pubKey = pKey.GetPublicKey();
            string pub58 = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";
            //  var base58 = Base58.DecodeFromBytes(pubKey.X.ToByteArray());
            output.WriteLine("pubKey " + pubKey);
            Assert.Equal(pub58, pubKey.ToString());
        }


        [Fact]
        public void GeneratePublicKey()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            PrivateKey pKey = new PrivateKey(privKey);          

            var pubKey = pKey.GetPublicKey();
            string pub58 = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";
            //  var base58 = Base58.DecodeFromBytes(pubKey.X.ToByteArray());
            output.WriteLine("pubKey " + pubKey);
            Assert.Equal(pub58, pubKey.ToString());
        }

    }
}