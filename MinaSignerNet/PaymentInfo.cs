using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class PaymentInfo
    {
        public string From { get; set; }
        public string To { get; set; }
        public BigInteger Amount { get; set; }
        public BigInteger Fee { get; set; }
        public BigInteger Nonce { get; set; }
    }
}
