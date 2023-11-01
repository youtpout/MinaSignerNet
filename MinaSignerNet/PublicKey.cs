using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class PublicKey
    {
        public bool IsOdd { get; set; }
        public BigInteger X { get; set; }
    }
}
