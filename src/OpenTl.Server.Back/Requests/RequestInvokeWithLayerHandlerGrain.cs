namespace OpenTl.Server.Back.Requests
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Server.Back.Contracts.Requests;

    public class RequestInvokeWithLayerHandlerGrain: BaseObjectHandlerGrain<RequestInvokeWithLayer, IObject>, IRequestInvokeWithLayerHandler
    {
        protected override Task<IObject> HandleProtected(Guid clientId, RequestInvokeWithLayer obj)
        {
            IObject data = new TConfig
                           {
                               CallConnectTimeoutMs = 1111,
                               DcOptions =  new TVector<IDcOption>(),
                               DisabledFeatures = new TVector<IDisabledFeature>(),
                               MeUrlPrefix = "q",
                               SuggestedLangCode = "q",
                           };
            
            return Task.FromResult(data);
        }
    }
}