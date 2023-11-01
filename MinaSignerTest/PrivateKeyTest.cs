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
        public void GenereateCorrectlyFromBase58()
        {
            string privKey = "EKDtctFSZuDJ8SXuWcbXHot57gZDtu7dNSAZNZvXek8KF8q6jV8K";
            PrivateKey pKey = new PrivateKey(privKey);
            output.WriteLine("decoding result : " + pKey.S.ToString());
        }

    }
}