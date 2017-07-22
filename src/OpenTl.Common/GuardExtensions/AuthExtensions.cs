namespace OpenTl.Common.GuardExtensions
{
    using BarsGroup.CodeGuard.Exceptions;
    using BarsGroup.CodeGuard.Internals;

    using Org.BouncyCastle.Math;

    public static class AuthExtensions
    {
        public static void IsValidDhGParameter(this ArgBase<BigInteger> arg, BigInteger prime)
        {
            var item = arg.Value;

            if (item.CompareTo(BigInteger.One) == -1 || prime.Subtract(BigInteger.One).CompareTo(item) == -1)
            {
                throw new OutOfRangeException<BigInteger>(item, BigInteger.One, prime.Subtract(BigInteger.One), "item");
            }
        }
        
        public static void IsValidDhPublicKey(this ArgBase<BigInteger> arg, BigInteger prime)
        {
            var key = arg.Value;

            if (key.CompareTo(BigInteger.Two.Pow(2048 - 64)) == -1 || prime.Subtract(BigInteger.Two.Pow(2048 - 64)).CompareTo(key) == -1)
            {
                throw new OutOfRangeException<BigInteger>(key, BigInteger.Two.Pow(2048 - 64), prime.Subtract(BigInteger.Two.Pow(2048 - 64)), "item");
            }
        }
    }
}