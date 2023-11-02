using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public static class BigintHelpers
    {
        public static BigInteger BytesToBigInt(byte[] bytes)
        {
            var x = BigInteger.Zero;
            int bitPosition = 0;
            foreach (var item in bytes)
            {
                x += new BigInteger(item) << bitPosition;
                bitPosition += 8;
            }
            return x;
        }

    }
}
