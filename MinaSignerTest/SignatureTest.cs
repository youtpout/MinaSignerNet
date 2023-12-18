using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using MinaSignerNet;
using MinaSignerNet.Models;
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
            // string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "4684693293117347807558860973481418471673384262972147112001047899275855117152";
            string r = "6159358923716484530342729010678563848064757356825548031847953789774052691328";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(message, privKey, Network.Testnet);

            Assert.Equal(BigInteger.Parse(s), signature.S);
            Assert.Equal(BigInteger.Parse(r), signature.R);
            output.WriteLine("signature " + signature.ToString());
            // Assert.Equal(signatureBase58, signature.ToString());

            var isGood = Signature.Verify(signature, message, pubKey, Network.Testnet);
            Assert.True(isGood);
        }

        [Fact]
        public void SignAuthString()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            string message = "Welcome to the mina asp auth, sign this message to authenticate b715ed91-2dfb-4d4b-a181-4a1257e3c293";
            // string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "28578775384902215803732967986618358826613085431816210932507129838504505976839";
            string r = "21006453149584518944039430027732441116924904780278919862860676381801052139241";
            string signBase58 = "7mXV946Kz1b7u2DK9KwAepQUwDyH98q5Z7H5Z3tZtBEDDWzQJuTVTAeBQGXuRk6x62Z6fRsjp7bRQWQ53WRYimEWQEpvU67y";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(message, privKey, Network.Testnet);

            Assert.Equal(BigInteger.Parse(s), signature.S);
            Assert.Equal(BigInteger.Parse(r), signature.R);
            output.WriteLine("signature " + signature.ToString());
            // Assert.Equal(signatureBase58, signature.ToString());

            var isGood = Signature.Verify(signature, message, pubKey, Network.Testnet);
            Assert.True(isGood);

            Assert.Equal(signBase58, signature.ToString());
        }

        [Fact]
        public void SignStringVerificationFailed()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            string message = "Hello world welcome in 2023 mina navigator programs";
            // string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "4684693293117347807558860973481418471673384262972147112001047899275855117152";
            string r = "6159358923716484530342729010678563848064757356825548031847953789774052691328";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";

            Signature signature = Signature.Sign(message, privKey, Network.Testnet);

            Assert.Equal(BigInteger.Parse(s), signature.S);
            Assert.Equal(BigInteger.Parse(r), signature.R);
            output.WriteLine("signature " + signature.ToString());
            // Assert.Equal(signatureBase58, signature.ToString());

            var isGood = Signature.Verify(signature, message + "different", pubKey, Network.Testnet);
            Assert.False(isGood);
        }

        [Fact]
        public void SignPayment()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            // string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "21741690141968527316842099905258655224998272675131623681001560160169122106253";
            string r = "28182486579702831857711270900219008886762343847884926345716879056365430444653";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";
            string toKey = "B62qkR9Har8apahum18KggGtHbAiumoQ65b6uH4vukaqdh3LZCA9jt5";

            var paymentInfo = new PaymentInfo()
            {
                Amount = 5,
                Fee = 1,
                Nonce = 0,
                From = pubKey,
                To = toKey,
                ValidUntil = 1702800000
            };

            Signature signature = Signature.SignPayment(paymentInfo, privKey, Network.Testnet);

            Assert.Equal(BigInteger.Parse(s), signature.S);
            Assert.Equal(BigInteger.Parse(r), signature.R);
            output.WriteLine("signature " + signature.ToString());
            // Assert.Equal(signatureBase58, signature.ToString());

            var isGood = Signature.VerifyPayment(signature, paymentInfo, pubKey, Network.Testnet);
            Assert.True(isGood);
        }

        [Fact]
        public void SignPaymentVerificationFailed()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            // string signatureBase58 = "7mXNcsg23PYDdziVuh2s9skr3fx3PV9UGxAtzRf4KwLmwVnypCPGwmUsRW6TmTKTLTP3KerhfdYWRLWtFGmFe2J6CF4GByvv";
            string s = "21741690141968527316842099905258655224998272675131623681001560160169122106253";
            string r = "28182486579702831857711270900219008886762343847884926345716879056365430444653";

            string pubKey = "B62qj5tBbE2xyu9k4r7G5npAGpbU1JDBkZm85WCVDMdCrHhS2v2Dy2y";
            string toKey = "B62qkR9Har8apahum18KggGtHbAiumoQ65b6uH4vukaqdh3LZCA9jt5";

            var paymentInfo = new PaymentInfo()
            {
                Amount = 5,
                Fee = 1,
                Nonce = 0,
                From = pubKey,
                To = toKey,
                ValidUntil = 1702800000
            };

            Signature signature = Signature.SignPayment(paymentInfo, privKey, Network.Testnet);

            Assert.Equal(BigInteger.Parse(s), signature.S);
            Assert.Equal(BigInteger.Parse(r), signature.R);
            output.WriteLine("signature " + signature.ToString());
            // Assert.Equal(signatureBase58, signature.ToString());

            paymentInfo = new PaymentInfo()
            {
                Amount = 5,
                Fee = 1,
                Nonce = 1,
                From = pubKey,
                To = toKey,
                ValidUntil = 1702800000
            };


            var isGood = Signature.VerifyPayment(signature, paymentInfo, pubKey, Network.Testnet);
            Assert.False(isGood);
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

        [Fact]
        public void Base58()
        {
            string base58 = "7mXFRCcMzD4Rzsmp5QQaHQFpHyDaEGrMExmu7hxSrjBXGAznVppDoFD763F8nNvrK7tsRyqRUqrKJPYFmV3eWnYs3ig4613H";
            var sign = new Signature(base58);

            Assert.Equal(base58, sign.ToString());
        }

    }
}