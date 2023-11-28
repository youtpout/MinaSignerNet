using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<bool> GetBitsLegacy()
        {
            var inputBits = Fields.SelectMany(x => x.BigIntToBytes(32).BytesToBits(255)).ToList();
            inputBits.AddRange(Bits);
            return inputBits;
        }

        public List<BigInteger> GetFieldsLegacy()
        {
            var packedFields = new List<BigInteger>();
            packedFields.AddRange(Fields);
            int index = 0;
            while (Bits.Count > index)
            {
                var fieldBits = Bits.Skip(index).Take(254).BitsToBytes().BytesToBigInt();
                var field = fieldBits;
                packedFields.Add(field);
                index += 254;
            }
            return packedFields;
        }

        public void Add(HashInputLegacy other)
        {
            this.Fields.AddRange(other.Fields);
            this.Bits.AddRange(other.Bits);
        }
    }
}
