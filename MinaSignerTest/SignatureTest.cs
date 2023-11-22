using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MinaSignerNet;
using System.Diagnostics;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

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
        public void SignMultipleMessage()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            List<BigInteger> messages = new List<BigInteger> { 2, 1 };
            string signatureBase58 = "7mXVMsyUxwX8NFKn9ppBJXPEUF2Hz6nyooPLosCw2GsrrcrW2WYvpZW7NUZAcY4sS9ZKDUNapRuSUW4nmCn919S197JWNqHk";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(messages, privKey, Network.Testnet);
            output.WriteLine("signature " + signature.ToString());
            Assert.Equal(signatureBase58, signature.ToString());

            var isGood = Signature.Verify(signature, messages, pubKey, Network.Testnet);
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
            string message = "Hello world welcome in 2023 mina navigator programs";
            string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "4684693293117347807558860973481418471673384262972147112001047899275855117152";
            string r = "6159358923716484530342729010678563848064757356825548031847953789774052691328";

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
        public void SignHash()
        {
            PrivateKey player1 = new PrivateKey("EKEdP3NMwmpiVae99xL2VHqoWaStzMAzgUu6txxcuK7E9euZHV5D");
            PrivateKey player2 = new PrivateKey("EKFdQ8aJ6jDViUKhQcaUrUAXcTuixCyBSFayS8xx9vCG8puUyGPG");
            UInt64 timestamp = 1699392007;
            BigInteger board = new BigInteger(70041);
            string signPlayer1 = "7mXLHdqQ3VefCRMRTNovtFuCrcxHhTonrr7uf2suGdq5w5VndVaU9qJC1N8XvkjDT7LiK68QzFvEoh5Sv57HVLzPvbBLszb6";
            string signPlayer2 = "7mXKuAv7aVew3GT6ttKrvrXeq7W6pqcizrK2EXH8EiUGvdrzq7er1oAEd1BGCXVaiGCwRfEY1ZZQrxFV1JePT27wfkrQoFvE";

            BigInteger expectedHash = BigInteger.Parse("5122390387187251027573396826656293360819060210216551388750089509715309485255");

            GameState state = new GameState()
            {
                Board = board,
                NextIsPlayer2 = true,
                StartTimeStamp = timestamp,
                Player1 = player1.GetPublicKey(),
                Player2 = player2.GetPublicKey()
            };
            Assert.Equal(expectedHash, state.Hash());

            Signature signature1 = Signature.Sign(state.Hash(), player1.ToString(), Network.Testnet);
            Signature signature2 = Signature.Sign(state.Hash(), player2.ToString(), Network.Testnet);
            Assert.Equal(signPlayer1, signature1.ToString());
            Assert.Equal(signPlayer2, signature2.ToString());

            var isGood = Signature.Verify(signature1, state.Hash(), player1.GetPublicKey().ToString(), Network.Testnet);
            Assert.True(isGood);
            isGood = Signature.Verify(signature2, state.Hash(), player2.GetPublicKey().ToString(), Network.Testnet);
            Assert.True(isGood);
        }

    }
}