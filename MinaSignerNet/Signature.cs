﻿using Blake2Core;
using MinaSignerNet.Models;
using MinaSignerNet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public class Signature
    {

        public const byte VersionNumber = 1;
        public const byte VersionByte = 154;
        public BigInteger R { get; set; }
        public BigInteger S { get; set; }

        public Signature()
        {

        }

        public Signature(string base58)
        {
            var decoded = base58.FromBase58Check(VersionByte);
            // we ignore the 2 first number who were version number and the last who is odd indicator
            R = decoded.Skip(1).Take(32).BytesToBigInt();
            S = decoded.Skip(33).Take(32).BytesToBigInt();
        }


        /// <summary>
        /// Sign a message from a private key
        /// </summary>        
        /// <param name="message">message to sign</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>Signature</returns>
        public static Signature Sign(string message, string privateKey, Network networkId = Network.Mainnet)
        {
            var msgByte = Encoding.UTF8.GetBytes(message);
            var bits = msgByte.Select(x =>
            {
                var list = new List<byte> { x };
                var byteTobit = list.BytesToBits();
                byteTobit.Reverse();
                return byteTobit;
            }).SelectMany(x => x).ToList();
            var input = new HashInputLegacy();
            input.Bits.AddRange(bits);
            return SignLegacy(input, privateKey, networkId);
        }


        /// <summary>
        /// Sign a message from a private key
        /// </summary>        
        /// <param name="message">bigIntger to sign</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>Signature</returns>
        public static Signature Sign(BigInteger message, string privateKey, Network networkId = Network.Mainnet)
        {
            return Sign(new List<BigInteger>() { message }, privateKey, networkId);
        }

        /// <summary>
        /// Sign a message from a private key
        /// </summary>        
        /// <param name="message">list bigIntger to sign</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>Signature</returns>
        public static Signature Sign(List<BigInteger> messages, string privateKey, Network networkId = Network.Mainnet)
        {
            var pKey = new PrivateKey(privateKey);
            var kPrime = DeriveNonce(messages, pKey, networkId);
            var groupPKey = Group.FromPrivateKey(pKey);
            var groupKPrime = Group.FromNonce(kPrime);
            var r = groupKPrime.X;
            var k = groupKPrime.Y.IsEven ? kPrime : FiniteField.Negate(kPrime, Constants.Q);
            var concat = new List<BigInteger>(messages);
            concat.Add(groupPKey.X);
            concat.Add(groupPKey.Y);
            concat.Add(r);
            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            var e = PoseidonHash.HashWithPrefix(prefix, concat);
            var s = FiniteField.Add(k, FiniteField.Mul(e, pKey.S, Constants.Q), Constants.Q);

            return new Signature() { R = r, S = s };
        }

        /// <summary>
        /// Sign a message from a private key
        /// </summary>        
        /// <param name="hashInput">hashInput to sign</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>Signature</returns>
        public static Signature SignLegacy(HashInputLegacy hashInput, string privateKey, Network networkId = Network.Mainnet)
        {
            var pKey = new PrivateKey(privateKey);
            var kPrime = DeriveNonceLegacy(hashInput, pKey, networkId);
            if (kPrime == BigInteger.Zero)
            {
                throw new Exception("sign: derived nonce is 0");
            }
            var groupPKey = Group.FromPrivateKey(pKey);
            var groupKPrime = Group.FromNonce(kPrime);
            var r = groupKPrime.X;
            var k = groupKPrime.Y.IsEven ? kPrime : FiniteField.Negate(kPrime, Constants.Q);
            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            var e = PoseidonHash.HashMessageLegacy(hashInput, pKey, r, networkId);
            var s = FiniteField.Add(k, FiniteField.Mul(e, pKey.S, Constants.Q), Constants.Q);

            return new Signature() { R = r, S = s };
        }


        /// <summary>
        /// Sign a payment transaction from a privateKey
        /// </summary>
        /// <param name="paymentInfo">payment info (from,to,price,amount,nonce)</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Signature SignPayment(PaymentInfo paymentInfo, string privateKey, Network networkId = Network.Mainnet)
        {
            var pKey = new PrivateKey(privateKey);
            UserCommand userCommand = new UserCommand(paymentInfo);
            var hashInput = userCommand.GetInputLegacy();
            return SignLegacy(hashInput, privateKey, networkId);
        }

        /// <summary>
        /// Sign a delegation transaction from a privateKey
        /// </summary>
        /// <param name="delegationInfo">delegation info (from,to,price,amount,nonce)</param>
        /// <param name="privateKey">private key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Signature SignStakeDelegation(DelegationInfo delegationInfo, string privateKey, Network networkId = Network.Mainnet)
        {
            var pKey = new PrivateKey(privateKey);
            UserCommand userCommand = new UserCommand(delegationInfo);
            var hashInput = userCommand.GetInputLegacy();
            return SignLegacy(hashInput, privateKey, networkId);
        }

        /// <summary>
        /// Verifies a signature created by SignPayment method, returns `true` if (and only if) the signature is valid. 
        /// </summary>
        /// <param name="signature">signature to check</param>
        /// <param name="paymentInfo">payment info (from,to,price,amount,nonce)</param>
        /// <param name="publicKey">public key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>True if correct</returns>
        public static bool VerifyPayment(Signature signature, PaymentInfo paymentInfo, string publicKey, Network networkId = Network.Mainnet)
        {
            UserCommand userCommand = new UserCommand(paymentInfo);
            var hashInput = userCommand.GetInputLegacy();
            return VerifyLegacy(signature, hashInput, publicKey, networkId);
        }

        /// <summary>
        /// Verifies a signature created by SignStakeDelegation method, returns `true` if (and only if) the signature is valid. 
        /// </summary>
        /// <param name="signature">signature to check</param>
        /// <param name="delegationInfo">delegation info (from,to,price,amount,nonce)</param>
        /// <param name="publicKey">public key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>True if correct</returns>
        public static bool VerifyStakeDelegation(Signature signature, DelegationInfo delegationInfo, string publicKey, Network networkId = Network.Mainnet)
        {
            UserCommand userCommand = new UserCommand(delegationInfo);
            var hashInput = userCommand.GetInputLegacy();
            return VerifyLegacy(signature, hashInput, publicKey, networkId);
        }

        /// <summary>
        /// Verifies a signature created by Sign method, returns `true` if (and only if) the signature is valid. 
        /// </summary>
        /// <param name="signature">signature to check</param>
        /// <param name="message">original bigIntger signed</param>
        /// <param name="publicKey">public key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>True if correct</returns>
        public static bool Verify(Signature signature, BigInteger message, string publicKey, Network networkId = Network.Mainnet)
        {
            return Verify(signature, new List<BigInteger> { message }, publicKey, networkId);
        }

        /// <summary>
        /// Verifies a signature created by Sign method, returns `true` if (and only if) the signature is valid. 
        /// </summary>
        /// <param name="signature">signature to check</param>
        /// <param name="message">original string signed</param>
        /// <param name="publicKey">public key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>True if correct</returns>
        public static bool Verify(Signature signature, string message, string publicKey, Network networkId = Network.Mainnet)
        {
            var msgByte = Encoding.UTF8.GetBytes(message);
            var bits = msgByte.Select(x =>
            {
                var list = new List<byte> { x };
                var byteTobit = list.BytesToBits();
                byteTobit.Reverse();
                return byteTobit;
            }).SelectMany(x => x).ToList();
            HashInputLegacy hashInput = new HashInputLegacy();
            hashInput.Bits.AddRange(bits);
            return VerifyLegacy(signature, hashInput, publicKey, networkId);
        }


        /// <summary>
        /// Verifies a signature created by Sign method, returns `true` if (and only if) the signature is valid. 
        /// </summary>
        /// <param name="signature">signature to check</param>
        /// <param name="messages">original list bigIntger signed</param>
        /// <param name="publicKey">public key in base58 format</param>
        /// <param name="networkId">network id by default we use mainnet</param>
        /// <returns>True if correct</returns>
        public static bool Verify(Signature signature, List<BigInteger> messages, string publicKey, Network networkId = Network.Mainnet)
        {
            var pubKey = new PublicKey(publicKey);
            var groupPubKey = Group.FromPublickKey(pubKey);
            var concat = new List<BigInteger>(messages);
            concat.Add(groupPubKey.X);
            concat.Add(groupPubKey.Y);
            concat.Add(signature.R);
            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            var e = PoseidonHash.HashWithPrefix(prefix, concat);
            var scale = EllipticCurve.ProjectiveScale(Constants.PallasGeneratorProjective, signature.S, Constants.P);
            var groupProj = new GroupProjective(groupPubKey);
            var scalePubKey = EllipticCurve.ProjectiveScale(groupProj, e, Constants.P);
            var R = EllipticCurve.ProjectiveSub(scale, scalePubKey, Constants.P);
            try
            {
                // if `R` is infinity, Group.fromProjective throws an error, so `verify` returns false
                Group grpFinal = R.ToGroup();
                return grpFinal.Y.IsEven && grpFinal.X == signature.R;
            }
            catch
            {
                return false;
            }
        }


        public static bool VerifyLegacy(Signature signature, HashInputLegacy messages, string publicKey, Network networkId = Network.Mainnet)
        {
            var pubKey = new PublicKey(publicKey);
            var groupPubKey = Group.FromPublickKey(pubKey);

            var e = PoseidonHash.HashMessageLegacy(messages, groupPubKey, signature.R, networkId);
            var scale = EllipticCurve.ProjectiveScale(Constants.PallasGeneratorProjective, signature.S, Constants.P);
            var groupProj = new GroupProjective(groupPubKey);
            var scalePubKey = EllipticCurve.ProjectiveScale(groupProj, e, Constants.P);
            var R = EllipticCurve.ProjectiveSub(scale, scalePubKey, Constants.P);
            try
            {
                // if `R` is infinity, Group.fromProjective throws an error, so `verify` returns false
                Group grpFinal = R.ToGroup();
                return grpFinal.Y.IsEven && grpFinal.X == signature.R;
            }
            catch
            {
                return false;
            }
        }

        public static BigInteger DeriveNonce(List<BigInteger> messages, PrivateKey privateKey, Network networkId)
        {
            var input = new HashInput();
            var group = Group.FromPrivateKey(privateKey);

            input.Fields.AddRange(messages);
            input.Fields.Add(group.X);
            input.Fields.Add(group.Y);
            input.Fields.Add(privateKey.S);

            List<BigInteger> dataPacked = new List<BigInteger>();
            input.Packed.Add(new Tuple<BigInteger, int>(new BigInteger((int)networkId), 8));


            var packedInput = input.PackToFields();
            var inputBits = packedInput.SelectMany(x => x.BigIntToBytes(32).BytesToBits().Take(255)).ToList();
            var inputBytes = inputBits.BitsToBytes();

            Blake2BConfig config = new Blake2BConfig() { OutputSizeInBytes = 32 };
            var outputBytes = Blake2B.ComputeHash(inputBytes.ToArray(), config);
            // drop the top two bits to convert into a scalar field element
            // (creates negligible bias because q = 2^254 + eps, eps << q)
            outputBytes[outputBytes.Count() - 1] &= 0x3f;

            return new BigInteger(outputBytes);
        }

        public static BigInteger DeriveNonceLegacy(HashInputLegacy message, PrivateKey privateKey, Network networkId)
        {
            var input = new HashInputLegacy();
            // don't pass it by ref, we need original value for hasmessage legacy
            input.Add(message);
            var group = Group.FromPrivateKey(privateKey);
            Debug.WriteLine(group.ToString());
            var publicKey = privateKey.GetPublicKey();
            var scalarBits = privateKey.S.BigIntToBytes(32).BytesToBits(255);

            var networkBytes = new List<Byte> { (byte)networkId };
            var idBits = networkBytes.BytesToBits(8);
            input.Bits.AddRange(scalarBits);
            input.Bits.AddRange(idBits);
            input.Fields.Add(group.X);
            input.Fields.Add(group.Y);
            //input.Fields.Add(privateKey.S);          


            //var packedInput = input.Bits.SelectMany(x=>x);
            var inputBits = input.GetBitsLegacy();
            var inputBytes = inputBits.BitsToBytes();

            Blake2BConfig config = new Blake2BConfig() { OutputSizeInBytes = 32 };
            var outputBytes = Blake2B.ComputeHash(inputBytes.ToArray(), config);
            // drop the top two bits to convert into a scalar field element
            // (creates negligible bias because q = 2^254 + eps, eps << q)
            outputBytes[outputBytes.Count() - 1] &= 0x3f;

            return new BigInteger(outputBytes);
        }

        public static BigInteger HashMessage(HashInput input, PublicKey publicKey, BigInteger r, Network networkId)
        {
            //var input = HashInput.append(message, { fields: [x, y, r] });
            //let prefix =
            //  networkId === 'mainnet'
            //    ? prefixes.signatureMainnet
            //    : prefixes.signatureTestnet;
            //return hashWithPrefix(prefix, packToFields(input));
            return new BigInteger();
        }

        public override string ToString()
        {
            var bytesR = R.BigIntToBytes(32);
            var bytesS = S.BigIntToBytes(32);
            List<byte> bytes = new List<byte>(bytesR.Concat(bytesS));
            // add version number in first place
            bytes.Insert(0, VersionNumber);

            return bytes.ToArray().ToBase58Check(VersionByte);
        }

    }
}
