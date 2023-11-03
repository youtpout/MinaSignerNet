using MinaSignerNet;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Xunit.Abstractions;

namespace MinaSignerTest
{
    public class ChecksumTest
    {
        private readonly ITestOutputHelper output;

        public ChecksumTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GenerateForKey()
        {
            byte[] input = new byte[]{
  90, 1, 25, 236, 110, 98, 84, 77, 68, 20, 188, 47, 223, 194, 116, 7, 158, 7,
    62, 234, 254, 74, 87, 108, 195, 72, 230, 220, 235, 230, 116, 21, 35, 14
     };
            byte[] checksum = new byte[] { 11, 104, 198, 56 };

            var calculChecksum = Base58.ComputeChecksum(input);
            Assert.Equal(checksum, calculChecksum);
        }


        [Fact]
        public void GenerateForSignature()
        {
            byte[] input = new byte[]{
  154, 1, 183, 152, 51, 12, 91, 104, 121, 135, 70, 58, 156, 243, 223, 87, 3,
  239, 221, 100, 60, 42, 12, 155, 4, 191, 134, 105, 14, 156, 178, 85, 27, 10,
  216, 136, 235, 173, 209, 28, 98, 193, 131, 45, 28, 158, 88, 197, 242, 249, 96,
  95, 193, 229, 213, 58, 80, 100, 125, 205, 43, 149, 253, 161, 153, 12
     };
            byte[] checksum = new byte[] { 139, 139, 172, 119 };

            var calculChecksum = Base58.ComputeChecksum(input);
            Assert.Equal(checksum, calculChecksum);
        }


        [Fact]
        public void generateMultiple()
        {
            byte[] input = new byte[]{
  154, 1, 183, 152, 51, 12, 91, 104, 121, 135, 70, 58, 156, 243, 223, 87, 3,
  239, 221, 100, 60, 42, 12, 155, 4, 191, 134, 105, 14, 156, 178, 85, 27, 10,
  216, 136, 235, 173, 209, 28, 98, 193, 131, 45, 28, 158, 88, 197, 242, 249, 96,
  95, 193, 229, 213, 58, 80, 100, 125, 205, 43, 149, 253, 161, 153, 12
     };
            byte[] checksum = new byte[] { 139, 139, 172, 119 };

            var calculChecksum = Base58.ComputeChecksum(input);
            Assert.Equal(checksum, calculChecksum);

            input = new byte[]{
  90, 1, 25, 236, 110, 98, 84, 77, 68, 20, 188, 47, 223, 194, 116, 7, 158, 7,
    62, 234, 254, 74, 87, 108, 195, 72, 230, 220, 235, 230, 116, 21, 35, 14
     };
            checksum = new byte[] { 11, 104, 198, 56 };
            calculChecksum = Base58.ComputeChecksum(input);
            Assert.Equal(checksum, calculChecksum);
        }

    }
}