using System;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using Org.BouncyCastle.Math;

namespace OpenTl.Common.Auth
{
    public static class ReqPqHelper
    {
        private static readonly Random Random = new Random();

        public static RequestReqPq Client(out byte[] nonce)
        {
            nonce = new byte[16];
            Random.NextBytes(nonce);
            
           return new RequestReqPq {Nonce = nonce};
        }
        
        public static TResPQ Server(byte[] nonce, long publicKeyFingerprint, out BigInteger p, out BigInteger q, out byte[] serverNonce)
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