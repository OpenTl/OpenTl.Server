/*
* Copyright ??(Michael J Steiner)
*
* This program is free software: you can redistribute it and/or modify it
* under the terms of version 3 of the GNU General Public License as
* published by the Free Software Foundation.
*
* This program is distributed in the hope that it will be useful, but
* WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
* General Public License for more details.
*
*  See http://www.gnu.org/licenses/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace OpenTl.Utils.Crypto
{
    public static class BigPrimality
    {
        private const int PrimesTableLimit = 1000;
        private static readonly BigInteger Two = new BigInteger(2);
        private static readonly BigInteger Three = new BigInteger(3);
        private static int[] PrimesTable;
        public static int SolovayStrassenComposites;
        public static int SolovayStrassenCandidates;
 
        public static int FermatComposites;
        public static int FermatCandidates;
 
        public static int MillerRabinComposites;
        public static int MillerRabinCandidates;
 
        public static int DeterministicComposites;
        public static int DeterministicCandidates;
 
        public static int Candidates;
        public static int Primes;
        private static readonly Random Random = new Random();
        public static double SolovayStrassenRatio => (double) SolovayStrassenComposites / SolovayStrassenCandidates * 100.0;
        public static double FermatRatio => (double) FermatComposites / FermatCandidates * 100.0;
        public static double MillerRabinRatio => (double) MillerRabinComposites / MillerRabinCandidates * 100.0;
        public static double DeterministicRatio => (double) DeterministicComposites / DeterministicCandidates * 100.0;
        public static double Ratio => (double) Primes / Candidates * 100.0;
 
        public static int Iterations { get; private set; }

        private static void BuildPrimesTable()
        {
            var tpt = new List<int>();
            for (var i = 3; i < PrimesTableLimit; ++i, ++i)
                if (Sieve(i))
                    tpt.Add(i);
 
            PrimesTable = tpt.ToArray();
        }
 
        /// <summary>
        ///     Not at all useful(efficient) for bit widths over 32 bits
        /// </summary>
        public static bool Sieve(BigInteger n)
        {
            var s = Sqrt(n);
            var a = Three;
            while (a < s)
            {
                if (n % a == 0)
                    return false;
                a += 2;
            }
            return true;
        }
 
        private static BigInteger Sqrt(this BigInteger number)
        {
            BigInteger n = 0, p = 0;
            if (number == BigInteger.Zero)
                return BigInteger.Zero;
            var high = number >> 1;
            var low = BigInteger.Zero;
            while (high > low + 1)
            {
                n = (high + low) >> 1;
                p = n * n;
                if (number < p)
                    high = n;
                else if (number > p)
                    low = n;
                else
                    break;
            }
            return number == p ? n : low;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Carmichael_number
        /// </summary>
        public static bool isCarmichael(this BigInteger n)
        {
            var factors = false;
            var s = Sqrt(n);
            var a = Three;
            var nmo = n - 1;
            while (a < n)
            {
                if (a > s && !factors)
                    return false;
                if (Gcd(a, n) > 1)
                    factors = true;
                else if (BigInteger.ModPow(a, nmo, n) != 1)
                    return false;
                a += 2;
            }
            return true;
        }
 
        public static List<BigInteger> GetFactors(this BigInteger n)
        {
            var Factors = new List<BigInteger>();
            var s = Sqrt(n);
            var a = Three;
            while (a < s)
            {
                if (n % a == 0)
                {
                    Factors.Add(a);
                    if (a * a != n)
                        Factors.Add(n / a);
                }
                a += 2;
            }
            return Factors;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Greatest_common_divisor
        /// </summary>
        private static BigInteger Gcd(this BigInteger a, BigInteger b)
        {
            while (b > BigInteger.Zero)
            {
                var r = a % b;
                a = b;
                b = r;
            }
            return a;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Least_common_multiple
        /// </summary>
        private static BigInteger Lcm(this BigInteger a, BigInteger b)
        {
            return a * b / Gcd(a, b);
        }
 
        /// <summary>
        ///     Get a pseudo prime number of bit length with a confidence(number of witnesses) level
        /// </summary>
        public static BigInteger GetPrime(int bitlength, int confidence = 2)
        {
            BigInteger candidate;
            bool f;
            Iterations = 0;
            do
            {
                Iterations++;
                candidate = NextRandomBigInt(3, GetMaxValue(bitlength), bitlength);
                f = IsPrime(candidate, confidence);
            } while (!f);
            return candidate;
        }

        private static BigInteger GetMaxValue(int bitLength)
        {
            return BigInteger.Pow(2, bitLength) - 1;
        }
        
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Primality_test
        ///     ~83% effective at finding composite numbers within the bit range 32 to 1024, +++?
        /// </summary>
        private static bool Deterministic(BigInteger candidate)
        {
            if (PrimesTable == null)
                BuildPrimesTable();
 
            DeterministicCandidates++;
            foreach (var p in PrimesTable)
                if (p < candidate)
                {
                    if (candidate % p == 0)
                    {
                        DeterministicComposites++;
                        return false;
                    }
                }
                else
                    break;
            return true;
        }
 
        public static bool IsPrime(this ulong bi, int confidence = 2)
        {
            return new BigInteger(bi).IsPrime(confidence);
        }
 
        public static bool IsPrime(this long bi, int confidence = 2)
        {
            return new BigInteger(bi).IsPrime(confidence);
        }
 
        public static bool IsPrime(this uint bi, int confidence = 2)
        {
            return new BigInteger(bi).IsPrime(confidence);
        }
 
        public static bool IsPrime(this int bi, int confidence = 2)
        {
            return new BigInteger(bi).IsPrime(confidence);
        }
 
        public static void ResetCompositeCounts()
        {
            SolovayStrassenComposites = 0;
            SolovayStrassenCandidates = 0;
 
            FermatComposites = 0;
            FermatCandidates = 0;
 
            MillerRabinComposites = 0;
            MillerRabinCandidates = 0;
 
            DeterministicComposites = 0;
            DeterministicCandidates = 0;
 
            Candidates = 0;
            Primes = 0;
        }
 
        /// <summary>
        ///     Check if a number is probably prime given a confidence level(Number of witnesses)
        /// </summary>
        public static bool IsPrime(this BigInteger candidate, int confidence = 2)
        {
            Candidates++;
            if (candidate == BigInteger.One)
                return false;
            if (candidate == Two)
                return true;
            if (candidate == Three)
                return true;
            if (candidate.IsEven)
                return false;
 
            if (!Deterministic(candidate))
                return false;
 
            if (!MillerRabin(candidate, confidence))
                return false;
 
            Primes++;
            return true;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Miller%E2%80%93Rabin_primality_test
        ///     100% effective at finding composite numbers with a confidence level of two, within the bit range 32 to 1024, +++?
        /// </summary>
        private static bool MillerRabin(BigInteger candidate, int confidence = 2)
        {
            MillerRabinCandidates++;
 
            var s = 0;
            var d = candidate - BigInteger.One;
            while ((d & 1) == 0)
            {
                d >>= 1;
                s++;
            }
            if (s == 0)
            {
                MillerRabinComposites++;
                return false;
            }
            var nmo = candidate - BigInteger.One;
            for (var i = 0; i < confidence; ++i)
            {
                var x = BigInteger.ModPow(NextRandomBigInt(2, nmo), d, candidate);
                if (x == 1 || x == nmo)
                    continue;
                int j;
                for (j = 1; j < s; ++j)
                {
                    x = BigInteger.ModPow(x, 2, candidate);
                    if (x == 1)
                    {
                        MillerRabinComposites++;
                        return false;
                    }
                    if (x == nmo)
                        break;
                }
                if (j == s)
                {
                    MillerRabinComposites++;
                    return false;
                }
            }
            return true;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Solovay%E2%80%93Strassen_primality_test
        /// </summary>
        private static bool SolovayStrassen(BigInteger candidate, int confidence = 2)
        {
            SolovayStrassenCandidates++;
            var cmo = candidate - 1;
            for (var i = 0; i < confidence; i++)
            {
                var a = NextRandomBigInt(2, cmo) % cmo + 1;
                var jacobian = (candidate + Jacobi(a, candidate)) % candidate;
                if (jacobian == 0 || BigInteger.ModPow(a, cmo >> 1, candidate) != jacobian)
                {
                    SolovayStrassenComposites++;
                    return false;
                }
            }
            return true;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Euler%E2%80%93Jacobi_pseudoprime
        /// </summary>
        private static int Jacobi(BigInteger a, BigInteger b)
        {
            if (b <= 0 || b % 2 == 0)
                return 0;
            var j = 1;
            if (a < 0)
            {
                a = -a;
                if (b % 4 == 3)
                    j = -j;
            }
            while (a != 0)
            {
                while (a % 2 == 0)
                {
                    a >>= 1;
                    if (b % 8 == 3 || b % 8 == 5)
                        j = -j;
                }
                var temp = a;
                a = b;
                b = temp;
                if (a % 4 == 3 && b % 4 == 3)
                    j = -j;
                a %= b;
            }
            return b == 1 ? j : 0;
        }
 
        /// <summary>
        ///     https://en.wikipedia.org/wiki/Fermat_pseudoprime
        ///     Watch for carmichael numbers (false positives)
        /// </summary>
        public static bool Fermat(this BigInteger candidate, int confidence = 2)
        {
            FermatCandidates++;
            var pmo = candidate - 1;
            for (var i = 0; i < confidence; i++)
                if (BigInteger.ModPow(NextRandomBigInt(2, pmo), pmo, candidate) != 1)
                {
                    FermatComposites++;
                    return false;
                }
            return true;
        }
 
        /// <summary>
        ///     A random number that meets the minimum and maximum limits.
        ///     Also ensures that the number is a positive odd number.
        /// </summary>
        private static BigInteger NextRandomBigInt(BigInteger min, BigInteger max, int bitlength = 0)
        {
            BigInteger n;
            var ByteLength = 0;
 
            if (bitlength == 0)
                ByteLength = max.ToByteArray().Length;
            else
                ByteLength = bitlength >> 3;
 
            var buffer = new byte[ByteLength];
            do
            {
                Random.NextBytes(buffer);
                n = new BigInteger(buffer);
            } while (n < min || n >= max || n.IsEven || n.Sign == -1);
            return n;
        }
    }
}