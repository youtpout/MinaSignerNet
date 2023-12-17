using System;
using System.Collections.Generic;
using System.Text;

namespace MinaSignerNet.Models
{
    public class Memo
    {
        private string memoValue;

        public string MemoValue
        {
            get { return memoValue; }
            set
            {
                var msgByte = Encoding.UTF8.GetBytes(value);
                if (msgByte.Length > 32)
                {
                    throw new Exception("Memo: string too long");
                }
                memoValue = value;
            }
        }

        public Memo(string data)
        {
            MemoValue = data;
        }

        public override string ToString()
        {
            var msgByte = Encoding.UTF8.GetBytes(MemoValue);
            var length = msgByte.Length;
            if (length > 32)
            {
                throw new Exception("Memo: string too long");
            }

            int input = 97;
            System.Text.ASCIIEncoding convertor = new System.Text.ASCIIEncoding();
            char output = convertor.GetChars(new byte[] { (byte)length })[0];

            StringBuilder last = new StringBuilder();
            int nb = 32 - length;
            if (nb > 0)
            {
                for (int i = 0; i < nb; i++)
                {
                    last.Append("\x00");
                }
            }

            return ($"\x01{output}{MemoValue}{last}");
        }
    }
}
