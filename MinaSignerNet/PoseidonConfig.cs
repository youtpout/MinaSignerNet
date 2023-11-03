using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class PoseidonConfig
    {
        public List<List<BigInteger>> Mds { get; set; }
        public List<List<BigInteger>> RoundConstants { get; set; }
        public int FullRounds { get; set; }
        public int partialRounds { get; set; }
        public bool HasInitialRoundConstant { get; set; }
        public int StateSize { get; set; }
        public int Rate { get; set; }
        public int Power { get; set; }
    }
}
