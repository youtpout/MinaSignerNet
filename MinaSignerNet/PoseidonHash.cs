using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using MinaSignerNet.Utils;

namespace MinaSignerNet
{
    public static class PoseidonHash
    {
        public static BigInteger HashWithPrefix(string prefix, List<BigInteger> input)
        {
            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
            var prefixBigInteger = PrefixToBigInteger(prefix);
            var list = new List<BigInteger> { prefixBigInteger };
            // salt prefix
            var init = PoseidonUpdate(initialState, list, PoseidonConstant.PoseidonConfigKimchiFp);
            return PoseidonUpdate(init, input, PoseidonConstant.PoseidonConfigKimchiFp).First();
        }

        public static BigInteger HashWithPrefixLegacy(string prefix, List<BigInteger> input)
        {
            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
            var prefixBigInteger = PrefixToBigInteger(prefix);
            var list = new List<BigInteger> { prefixBigInteger };
            // salt prefix
            var init = PoseidonUpdate(initialState, list, PoseidonConstant.PoseidonConfigLegacyFp);
            return PoseidonUpdate(init, input,PoseidonConstant.PoseidonConfigLegacyFp).First();
        }

        public static BigInteger HashMessageLegacy(HashInputLegacy input, PrivateKey privateKey, BigInteger r, Network networkId)
        {
            var group = Group.FromPrivateKey(privateKey);
            input.Fields.Add(group.X);
            input.Fields.Add(group.Y);
            input.Fields.Add(r);

            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            return HashWithPrefixLegacy(prefix, input.GetFieldsLegacy());
        }

        public static BigInteger HashMessageLegacy(List<bool> messages, Group groupPublicKey, BigInteger r, Network networkId)
        {
            var input = new HashInputLegacy();
            input.Bits.AddRange(messages);
            input.Fields.Add(groupPublicKey.X);
            input.Fields.Add(groupPublicKey.Y);
            input.Fields.Add(r);

            var prefix = networkId == Network.Mainnet ? Constants.SignatureMainnet : Constants.SignatureTestnet;
            return HashWithPrefixLegacy(prefix, input.GetFieldsLegacy());
        }

        public static BigInteger Hash(List<BigInteger> input)
        {
            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
            return PoseidonUpdate(initialState, input, PoseidonConstant.PoseidonConfigKimchiFp).First();
        }

        public static BigInteger PrefixToBigInteger(string prefix)
        {
            int fieldSize = 32;
            if (prefix.Length >= fieldSize)
                throw new Exception("prefix too long");

            var stringBytes = Encoding.UTF8.GetBytes(prefix);

            var array = new byte[fieldSize];
            stringBytes.CopyTo(array, 0);

            return array.BytesToBigInt();
        }

        public static List<BigInteger> PoseidonUpdate(List<BigInteger> state, List<BigInteger> input, PoseidonConfig config)
        {
            if (input.Count == 0)
            {
                Permutation(state, config);
                return state;
            }
            // pad input with zeros so its length is a multiple of the rate
            decimal n = Math.Ceiling((decimal)input.Count / config.Rate) * config.Rate;
            var array = new BigInteger[(int)n];
            input.CopyTo(array, 0);
            // for every block of length `rate`, add block to the first `rate` elements of the state, and apply the permutation
            for (var blockIndex = 0; blockIndex < n; blockIndex += config.Rate)
            {
                for (var i = 0; i < config.Rate; i++)
                {
                    state[i] = FiniteField.Add(state[i], array[blockIndex + i], Constants.P);
                }
                Permutation(state, config);
            }

            return state;
        }


        public static void Permutation(List<BigInteger> state, PoseidonConfig config)
        {
            // special case: initial round constant
            var offset = 0;
            if (config.HasInitialRoundConstant)
            {
                for (var i = 0; i < config.StateSize; i++)
                {
                    state[i] = FiniteField.Add(state[i], config.RoundConstants[0][i], Constants.P);
                }
                offset = 1;
            }
            for (var round = 0; round < config.FullRounds; round++)
            {
                // raise to a power
                for (var i = 0; i < config.StateSize; i++)
                {
                    state[i] = FiniteField.Power(state[i], config.Power, Constants.P);
                }
                var oldState = new List<BigInteger>(state);
                for (var i = 0; i < config.StateSize; i++)
                {
                    // multiply by mds matrix
                    state[i] = FiniteField.Dot(config.Mds[i], oldState, Constants.P);
                    // add round constants
                    state[i] = FiniteField.Add(state[i], config.RoundConstants[round + offset][i], Constants.P);
                }
            }
        }


    }
}
