using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class HashInputLegacy
    {
        public List<BigInteger> Fields { get; set; }
        public List<bool> Bits { get; set; }

        public HashInputLegacy()
        {
            Fields = new List<BigInteger>();
            Bits = new List<bool>();
        }
    }
}
