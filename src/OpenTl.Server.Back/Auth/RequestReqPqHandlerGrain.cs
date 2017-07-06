using System;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Helpers;
using Orleans.Runtime;
using BigInteger = OpenTl.Utils.Crypto.BigInteger;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqPqHandlerGrain:  BaseObjectHandlerGrain<RequestReqPq, TResPQ>, IRequestReqPqHandler
    {
        private  static  readonly Random Random = new Random();

        protected override Task<TResPQ> HandleProtected(RequestReqPq obj)
        {
            var serverNonce = new byte[16];
            Random.NextBytes(serverNonce);

            var p = GeneratePrime();
            var q = GeneratePrime();
            GetLogger().Info($"P = {p} Q = {q}");
            
            var pq = p * q;
            
            return Task.FromResult(new TResPQ
            {
                Nonce = obj.Nonce,
                ServerNonce = serverNonce,
                Pq = SerializationUtils.GetString(pq.getBytes()),
                ServerPublicKeyFingerprints = new TVector<long>(RsaHelper.PublicKeyFingerprint)
            });
        }

        private static BigInteger GeneratePrime()
        {
            BigInteger p;
            BigInteger moreSec;
            do
            {
                p = BigInteger.genPseudoPrime(31, 100, Random);
                moreSec = ((p - 1) / 2);
            } while (moreSec.isProbablePrime(100));

            return p;
        }
    }
}