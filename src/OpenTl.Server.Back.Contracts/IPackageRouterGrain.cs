using System;
using System.Threading.Tasks;

namespace OpenTl.Server.Back.Contracts
{
    public interface IPackageRouterGrain: Orleans.IGrainWithIntegerKey
    {
        Task<Tuple<Guid?, byte[]>> Handle(Guid keyId, byte[] request);
    }
}