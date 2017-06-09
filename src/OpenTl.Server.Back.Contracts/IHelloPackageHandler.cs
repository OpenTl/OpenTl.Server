using System;
 using System.Threading.Tasks;

namespace OpenTl.Server.Back.Contracts
{
    public interface IHelloPackageHandler : Orleans.IGrainWithIntegerKey, IPackageHandler
    {
    }
}