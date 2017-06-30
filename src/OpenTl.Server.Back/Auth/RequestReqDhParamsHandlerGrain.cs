using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Server.Back.Contracts.Auth;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqDhParamsHandlerGrain:  BaseObjectHandlerGrain<RequestReqDHParams, IServerDHParams>, IRequestReqDhParamsHandler
    {
        protected override Task<IServerDHParams> HandleProtected(RequestReqDHParams obj)
        {
            return null;
        }
    }
}