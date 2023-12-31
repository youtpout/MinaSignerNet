﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet.Models
{
    public class DelegationInfo
    {
        public string From { get; set; }
        public string To { get; set; }
        public UInt64 Fee { get; set; }
        public UInt32 Nonce { get; set; }
        public string Memo { get; set; }
        public UInt32 ValidUntil { get; set; }

        public DelegationInfo()
        {
            Memo = string.Empty;
        }
    }
}
