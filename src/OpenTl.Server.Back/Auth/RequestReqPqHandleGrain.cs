using System;
using System.Text;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Crypto;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqPqHandleGrain:  BaseObjectHandlerGrain<RequestReqPq, TResPQ>, IRequestReqPqHandler
    {
        private  static  readonly Random Random = new Random();

        protected override Task<TResPQ> HandleProtected(RequestReqPq obj)
        {
            var serverNonce = new byte[16];
            Random.NextBytes(serverNonce);

            var prime = BigInteger.ProbablePrime(63, Random).ToByteArrayUnsigned();

            return Task.FromResult(new TResPQ
            {
                Nonce = obj.Nonce,
                ServerNonce = serverNonce,
                Pq = Encoding.UTF8.GetString(prime),
                ServerPublicKeyFingerprints = new TVector<long>(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
            });
        }
    }
}