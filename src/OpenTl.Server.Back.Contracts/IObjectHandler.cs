using System;
using System.Threading.Tasks;
using OpenTl.Schema;

namespace OpenTl.Server.Back.Contracts
{
    public interface IObjectHandler : Orleans.IGrainWithIntegerKey
    {
        Task<IObject> Handle(Guid clientId, IObject obj);
    }
}