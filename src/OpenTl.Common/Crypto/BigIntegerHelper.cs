namespace OpenTl.Common.Crypto
{
    using System;

    using Org.BouncyCastle.Math;

    public class BigIntegerHelper
    {
        private static readonly BigInteger Maxlong = BigInteger.ValueOf(long.MaxValue);

        private static readonly BigInteger Bigtwo = BigInteger.ValueOf(2);

        public static BigInteger SmallestPrimeFactor(BigInteger n)
        {
            if (n.CompareTo(BigInteger.One) <= 0)
            {
                return BigInteger.Zero;
            }
            if (n.Mod(Bigtwo).Equals(BigInteger.Zero))
            {
                return Bigtwo;
            }
            return n.CompareTo(Maxlong) <= 0
                       ? BigInteger.ValueOf(SmallestPrimeFactor(n.LongValue))
                       : InternalPrimeFactor(n);

            // use special BigInteger code for this...
        }

        private static long SmallestPrimeFactor(long n)
        {
            if (n < 1)
            {
                return 0;
            }
            if (n % 2 == 0)
            {
                return 2;
            }

            // only need aproximate end-point... even if n is larger than 48 bits
            // (the largest accurate integer number in double),
            // the sqrt will be only off by 1 at most.
            var root = (long)(Math.Sqrt(n)) + 1;
            for (long i = 3; i <= root; i += 2)
            {
                if (n % i == 0)
                {
                    return i;
                }
            }

            // it's prime!
            return n;
        }

        /**
         * From http://faruk.akgul.org/blog/javas-missing-algorithm-biginteger-sqrt/
         */
        public static BigInteger Sqrt(BigInteger n)
        {
            var a = BigInteger.One;
            var b = new BigInteger(n.ShiftRight(5).Add(new BigInteger("8")).ToString());
            while (b.CompareTo(a) >= 0)
            {
                var mid = new BigInteger(a.Add(b).ShiftRight(1).ToString());
                if (mid.Multiply(mid).CompareTo(n) > 0)
                    b = mid.Subtract(BigInteger.One);
                else
                    a = mid.Add(BigInteger.One);
            }
            return a.Subtract(BigInteger.One);
        }

        private static BigInteger InternalPrimeFactor(BigInteger n)
        {
            // only call this method when n > long.MaxValue
            // See this SO Question: http://stackoverflow.com/a/4407884/1305253
            // and using the code from this blog: http://faruk.akgul.org/blog/javas-missing-algorithm-biginteger-sqrt/
            var root = Sqrt(n);

            // keep a limit in `long` space, swap to BigInteger space when we need to.
            var maxlimit = root.CompareTo(Maxlong) > 0
                                ? long.MaxValue
                                : root.LongValue;

            // only need to start from 3, 2 has been dealt with....

            for (long i = 3; i <= maxlimit; i += 2)
            {
                if (n.Mod(BigInteger.ValueOf(i)).Equals(BigInteger.Zero))
                {
                    return BigInteger.ValueOf(i);
                }
            }

            // OK, no prime factor found less than long.
            // resort to using BigInteger arithmatic in the loop.
            // MAXLONG is odd, we can go from there.
            for (var i = Maxlong; root.CompareTo(i) >= 0; i = i.Add(Bigtwo))
            {
                if (n.Mod(i).Equals(BigInteger.Zero))
                {
                    return i;
                }
            }

            // it's prime!
            return n;
        }
    }
}