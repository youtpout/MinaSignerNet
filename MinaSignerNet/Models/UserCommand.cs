﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MinaSignerNet.Utils;

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
                Memo = new Memo(paymentInfo.Memo),
                Nonce = paymentInfo.Nonce,
                ValidUntil = paymentInfo.ValidUntil
            };

        }

        public UserCommand(DelegationInfo delegationInfo)
        {
            Body = new Body()
            {
                Receiver = new PublicKey(delegationInfo.To),
                Source = new PublicKey(delegationInfo.From),
                Tag = TagEnum.StakeDelegation
            };

            Common = new Common()
            {
                Fee = delegationInfo.Fee,
                FeePayer = new PublicKey(delegationInfo.From),
                Memo = new Memo(delegationInfo.Memo),
                Nonce = delegationInfo.Nonce,
                ValidUntil = delegationInfo.ValidUntil
            };

        }

        public HashInputLegacy GetInputLegacy()
        {
            // common
            var feeBits = Common.Fee.ToBits();
            var legacyBits = Constants.LegacyTokenId;
            var feePayer = Common.FeePayer.ToHashInputLegacy();
            var nonce = Common.Nonce.ToBits();
            var validUntil = Common.ValidUntil.ToBits();
            var memo = Common.Memo.ToBits();

            var hashInputCommon = new HashInputLegacy();
            hashInputCommon.Bits.AddRange(feeBits);
            hashInputCommon.Bits.AddRange(legacyBits);
            hashInputCommon.Add(feePayer);
            hashInputCommon.Bits.AddRange(nonce);
            hashInputCommon.Bits.AddRange(validUntil);
            hashInputCommon.Bits.AddRange(memo);

            // body
            var tag = Body.Tag.ToBits();
            var from = Body.Source.ToHashInputLegacy();
            var to = Body.Receiver.ToHashInputLegacy();
            var amount = Body.Amount.ToBits();

            var hashInputBody = new HashInputLegacy();
            hashInputBody.Bits.AddRange(tag);
            hashInputBody.Add(from);
            hashInputBody.Add(to);
            hashInputBody.Bits.AddRange(legacyBits);
            hashInputBody.Bits.AddRange(amount);
            hashInputBody.Bits.Add(false);  // token_locked


            hashInputCommon.Add(hashInputBody);
            return hashInputCommon;
        }
    }
}
