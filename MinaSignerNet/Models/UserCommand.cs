using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MinaSignerNet.Models
{
    public class UserCommand
    {
        public Common Common { get; set; }
        public Body Body { get; set; }

        public UserCommand() { }

        public UserCommand(PaymentInfo paymentInfo)
        {
            Body = new Body()
            {
                Amount = paymentInfo.Amount,
                Receiver = new PublicKey(paymentInfo.To),
                Source = new PublicKey(paymentInfo.From),
                Tag = TagEnum.Payment
            };

            Common = new Common()
            {
                Fee = paymentInfo.Fee,
                FeePayer = new PublicKey(paymentInfo.From),
                Memo = paymentInfo.Memo,
                Nonce = paymentInfo.Nonce,
                ValidUntil = paymentInfo.ValidUntil
            };

        }

        public BigInteger GetInputLegacy()
        {
            var array = new List<bool>();
            var feeBits = new BigInteger(Common.Fee).BigIntToBytes(255).BytesToBits();
            var legacyBits = Constants.LegacyTokenId;
            var key = Common.FeePayer;
           
            // todo implement hashinputlegacy
            return new BigInteger();
        }
    }
}
