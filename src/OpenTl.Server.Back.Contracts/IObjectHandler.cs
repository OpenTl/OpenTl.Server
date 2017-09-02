using System;
using System.Threading.Tasks;

namespace OpenTl.Server.Back.Contracts
{
    public interface IObjectHandler : Orleans.IGrainWithIntegerKey
    {
        Task<byte[]> Handle(ulong keyId, byte[] package);    
    }
}