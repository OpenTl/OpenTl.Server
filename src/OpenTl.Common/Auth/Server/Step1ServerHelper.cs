namespace OpenTl.Common.Auth.Server
{
    using System;

    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Org.BouncyCastle.Math;

    public static class Step1ServerHelper
    {
        private static readonly Random Random = new Random();

        public static TResPQ GetResponse(byte[] nonce, long publicKeyFingerprint, out BigInteger p, out BigInteger q, out byte[] serverNonce)
        {
            p =  BigInteger.ProbablePrime(28, Random);
            q =  BigInteger.ProbablePrime(28, Random);

            var pq = p.Multiply(q).ToByteArray();
            
            serverNonce = new byte[16];
            Random.NextBytes(serverNonce);

            return new TResPQ
            {
                Nonce = nonce,
                ServerNonce = serverNonce,
                Pq = SerializationUtils.GetStringFromBinary(pq),
                ServerPublicKeyFingerprints = new TVector<long>(publicKeyFingerprint)
            };
        }
    }
}