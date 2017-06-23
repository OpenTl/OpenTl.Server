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
    public class PackageRouterGrain : Grain, IPackageRouterGrain
    {
        private readonly Dictionary<Type, IObjectHandler> _handlers = new Dictionary<Type, IObjectHandler>();

        public override Task OnActivateAsync()
        {
            _handlers[typeof(RequestReqPq)] = GrainFactory.GetGrain<IRequestReqPqHandler>(0);

            return base.OnActivateAsync();
        }

        public async Task<byte[]> Handle(byte[] package)
        {
            var obj = Serializer.DeserializeObject(package);

            var responce = await _handlers[obj.GetType()].Handle(obj);

            return Serializer.SerializeObject(responce).ToArray();
        }
    }
}