using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class HashInput
    {
        public List<BigInteger> Fields { get; set; }
        public List<Tuple<BigInteger,int>> Packed { get; set; }

        public HashInput()
        {
            Fields = new List<BigInteger>();
            Packed = new List<Tuple<BigInteger, int>>();
        }
    }
}
