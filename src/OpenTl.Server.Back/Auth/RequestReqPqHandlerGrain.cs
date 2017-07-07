using System;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Server.Back.Cache;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Helpers;
using OpenTl.Utils.Crypto;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqPqHandlerGrain:  BaseObjectHandlerGrain<RequestReqPq, TResPQ>, IRequestReqPqHandler
    {
        private  static  readonly Random Random = new Random();

        protected override Task<TResPQ> HandleProtected(Guid clientId, RequestReqPq obj)
        {
            var cache = AuthCache.NewAuthCache(clientId);
            
            var serverNonce = new byte[16];
            Random.NextBytes(serverNonce);

            cache.ServerNonce = serverNonce;

            GenerateKeys(cache, out var p, out var q);

            var pq = p * q;
            
            return Task.FromResult(new TResPQ
            {
                Nonce = obj.Nonce,
                ServerNonce = serverNonce,
                Pq = SerializationUtils.GetStringFromBinary(pq.ToByteArray()),
                ServerPublicKeyFingerprints = new TVector<long>(RsaHelper.PublicKeyFingerprint)
            });
        }

        private static void GenerateKeys(AuthCache cache, out BigInteger p, out BigInteger q)
        {
            p = GeneratePrime();
            cache.P = p;

            q = GeneratePrime();
            cache.Q = q;
        }

        private static BigInteger GeneratePrime()
        {
            BigInteger p;
            BigInteger moreSec;
            do
            {
                p = BigPrimality.GetPrime(28, 100);
                moreSec = ((p - 1) / 2);
            } while (moreSec.IsPrime(100));

            return p;
        }
    }
}