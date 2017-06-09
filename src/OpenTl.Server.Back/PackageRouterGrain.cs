using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenTl.Server.Back.Contracts;
using Orleans;
using Orleans.Runtime;

namespace OpenTl.Server.Back
{
    public class PackageRouterGrain : Grain, IPackageRouterGrain
    {
        readonly Dictionary<int, IPackageHandler> _handlers = new Dictionary<int, IPackageHandler>();

        public override Task OnActivateAsync()
        {
            _handlers[1] = GrainFactory.GetGrain<IHelloPackageHandler>(0);

            return base.OnActivateAsync();
        }

        public async Task<byte[]> Handle(byte[] package)
        {
            using (var stream = new MemoryStream(package))
            using (var reader = new BinaryReader(stream))
            {
                var packageId = reader.ReadInt32();

                return await _handlers[packageId].Handle(package);
            }
        }
    }
}