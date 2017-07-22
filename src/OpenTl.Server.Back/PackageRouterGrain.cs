using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Server.Back.Contracts;
using OpenTl.Server.Back.Contracts.Auth;
using Orleans;

namespace OpenTl.Server.Back
{
    using System.Linq;

    public class PackageRouterGrain : Grain, IPackageRouterGrain
    {
        private readonly Dictionary<uint, IObjectHandler> _handlers = new Dictionary<uint, IObjectHandler>();

        public override Task OnActivateAsync()
        {
            _handlers[1615239032] = GrainFactory.GetGrain<IRequestReqPqHandler>(0);
            _handlers[3608339646] = GrainFactory.GetGrain<IRequestReqDhParamsHandler>(0);
            _handlers[4110704415] = GrainFactory.GetGrain<IRequestSetClientDhParamsHandler>(0);

            return base.OnActivateAsync();
        }

        public async Task<byte[]> Handle(Guid clientId, byte[] package)
        {
            var objectId = BitConverter.ToUInt32(package, 0);

            var responce = await _handlers[objectId].Handle(clientId, package);

            return responce;
        }
    }
}