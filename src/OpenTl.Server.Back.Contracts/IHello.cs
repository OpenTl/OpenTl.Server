using System;
 using System.Threading.Tasks;

namespace OpenTl.Server.Back.Contracts
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}