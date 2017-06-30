using System;
using System.Text;
using System.Threading.Tasks;
using BarsGroup.CodeGuard;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Crypto;
using OpenTl.Server.Back.Helpers;
using Orleans.Runtime;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqPqHandlerGrain:  BaseObjectHandlerGrain<RequestReqPq, TResPQ>, IRequestReqPqHandler
    {
        private  static  readonly Random Random = new Random();

        protected override Task<TResPQ> HandleProtected(RequestReqPq obj)
        {
            var serverNonce = new byte[16];
            Random.NextBytes(serverNonce);

            var prime = BigInteger.ProbablePrime(63, Random).ToByteArray();
            
            return Task.FromResult(new TResPQ
            {
                Nonce = obj.Nonce,
                ServerNonce = serverNonce,
                Pq = SerializationUtils.GetString(prime),
                ServerPublicKeyFingerprints = new TVector<long>(RsaHelper.PublicKeyFingerprint)
            });
        }
    }
}