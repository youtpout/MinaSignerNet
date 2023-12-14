using System;
using System.Collections.Generic;
using System.Text;

namespace MinaSignerNet.Models
{
    public class Common
    {
        public UInt64 Fee { get; set; }
        public PublicKey FeePayer { get; set; }
        public UInt32 Nonce { get; set; }
        public UInt32 ValidUntil { get; set; }
        public string Memo { get; set; }
    }
}
