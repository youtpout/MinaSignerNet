using MinaSignerNet;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;

namespace MinaSignerTest
{
    public class Base58Test
    {
        private readonly ITestOutputHelper output;

        public Base58Test(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void EncodeCorrectly()
        {
            string input = "HelloWorld";
            string expected = "54uZdajEaDdN6F";
            string result = Base58.EncodeFromString(input);
            output.WriteLine("encoding result : " + result);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DecodeCorrectly()
        {
            string input = "C7tDXZnoyP9";
            string expected = "ByeWorld";
            string result = Base58.DecodeToString(input);
            output.WriteLine("decoding result : " + result);
            Assert.Equal(expected, result);
        }
    }
}