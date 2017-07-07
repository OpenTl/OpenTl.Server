using System;
using System.Numerics;

namespace OpenTl.Utils.Crypto
{
    public class FactorizedPair
    {
        private readonly BigInteger _p;
        private readonly BigInteger _q;

        public FactorizedPair(BigInteger p, BigInteger q)
        {
            this._p = p;
            this._q = q;
        }

        public FactorizedPair(long p, long q)
        {
            this._p = new BigInteger(p);
            this._q = new BigInteger(q);
        }

        public BigInteger Min => BigInteger.Min(_p, _q);

        public BigInteger Max => BigInteger.Max(_p, _q);

        public override string ToString()
        {
            return $"P: {_p}, Q: {_q}";
        }
    }

    public class Factorizator
    {
        private static readonly Random Random = new Random();

        private static BigInteger FindSmallMultiplierLopatin(BigInteger what)
        {
            BigInteger g = 0;
            for (var i = 0; i < 3; i++)
            {
                var q = (Random.Next(128) & 15) + 17;
                BigInteger x = Random.Next(1000000000) + 1, y = x;
                var lim = 1 << (i + 18);
                for (var j = 1; j < lim; j++)
                {
                    BigInteger a = x, b = x, c = q;
                    while (b != 0)
                    {
                        if ((b & 1) != 0)
                        {
                            c += a;
                            if (c >= what)
                                c -= what;
                        }
                        a += a;
                        if (a >= what)
                            a -= what;
                        b >>= 1;
                    }
                    x = c;
                    var z = x < y ? y - x : x - y;
                    g = Gcd(z, what);
                    if (g != 1)
                        break;
                    if ((j & (j - 1)) == 0)
                        y = x;
                }
                if (g > 1)
                    break;
            }

            var p = what / g;
            return BigInteger.Min(p, g);
        }

        private static BigInteger Gcd(BigInteger a, BigInteger b)
        {
            while (a != 0 && b != 0)
            {
                while ((b & 1) == 0)
                    b >>= 1;
                while ((a & 1) == 0)
                    a >>= 1;
                if (a > b)
                    a -= b;
                else b -= a;
            }
            return b == 0 ? a : b;
        }

        public static FactorizedPair Factorize(BigInteger pq)
        {
            if (pq.ToByteArray().Length <= 8)
            {
                var divisor = FindSmallMultiplierLopatin(pq);
                return new FactorizedPair(divisor, pq / divisor);
            }
            // TODO: port pollard factorization
            throw new InvalidOperationException("pq too long; TODO: port the pollard algo");
            // logger.error("pq too long; TODO: port the pollard algo");
            // return null;
        }
    }
}