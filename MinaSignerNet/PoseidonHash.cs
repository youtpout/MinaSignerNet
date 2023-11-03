﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace MinaSignerNet
{
    public static class PoseidonHash
    {
        public static PoseidonConfig PoseidonConfigKimchiFp;
        static PoseidonHash()
        {
            var txt = File.ReadAllText("PoseidonParamsKimchiFp.json");
            PoseidonConfigKimchiFp = JsonConvert.DeserializeObject<PoseidonConfig>(txt);
        }

        public static BigInteger HashWithPrefix(string prefix, List<BigInteger> input)
        {
            List<BigInteger> initialState = new List<BigInteger> { BigInteger.Zero, BigInteger.Zero, BigInteger.Zero };
            var prefixBigInteger = PrefixToBigInteger(prefix);
            var list = new List<BigInteger> { prefixBigInteger };
            // salt prefix
            var init = PoseidonUpdate(initialState, list);
            return PoseidonUpdate(init, input).First();
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

        public static List<BigInteger> PoseidonUpdate(List<BigInteger> state, List<BigInteger> input)
        {
            if (input.Count == 0)
            {
                Permutation(state, PoseidonConfigKimchiFp);
                return state;
            }
            // pad input with zeros so its length is a multiple of the rate
            decimal n = Math.Ceiling((decimal)input.Count / PoseidonConfigKimchiFp.Rate) * PoseidonConfigKimchiFp.Rate;
            var array = new BigInteger[(int)n];
            input.CopyTo(array, 0);
            // for every block of length `rate`, add block to the first `rate` elements of the state, and apply the permutation
            for (var blockIndex = 0; blockIndex < n; blockIndex += PoseidonConfigKimchiFp.Rate)
            {
                for (var i = 0; i < PoseidonConfigKimchiFp.Rate; i++)
                {
                    state[i] = FiniteField.Add(state[i], array[blockIndex + i],Constants.P);
                }
                Permutation(state, PoseidonConfigKimchiFp);
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