using System;

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

        public BigInteger Min => _p.min(_q);

        public BigInteger Max => _p.max(_q);

        public override string ToString()
        {
            return $"P: {_p}, Q: {_q}";
        }
    }

    public class Factorizator
    {
        private static readonly Random Random = new Random();

        private static long FindSmallMultiplierLopatin(long what)
        {
            long g = 0;
            for (var i = 0; i < 3; i++)
            {
                var q = (Random.Next(128) & 15) + 17;
                long x = Random.Next(1000000000) + 1, y = x;
                var lim = 1 << (i + 18);
                for (var j = 1; j < lim; j++)
                {
                    long a = x, b = x, c = q;
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
            return Math.Min(p, g);
        }

        private static long Gcd(long a, long b)
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
            if (pq.bitCount() < 64)
            {
                var pqlong = pq.LongValue();
                var divisor = FindSmallMultiplierLopatin(pqlong);
                return new FactorizedPair(new BigInteger(divisor), new BigInteger(pqlong / divisor));
            }
            // TODO: port pollard factorization
            throw new InvalidOperationException("pq too long; TODO: port the pollard algo");
            // logger.error("pq too long; TODO: port the pollard algo");
            // return null;
        }
    }
}