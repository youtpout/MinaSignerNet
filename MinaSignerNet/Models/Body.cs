using System;
using System.Collections.Generic;
using System.Text;

namespace MinaSignerNet.Models
{

    public enum TagEnum
    {
        Payment,
        StakeDelegation
    }

    public class Body
    {
        public TagEnum Tag { get; set; }
        public PublicKey Source { get; set; }
        public PublicKey Receiver { get; set; }
        public UInt64 Amount { get; set; }
    }
}
