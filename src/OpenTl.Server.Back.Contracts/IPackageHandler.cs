using System.Threading.Tasks;

namespace OpenTl.Server.Back.Contracts
{
    public interface IPackageHandler
    {
        Task<byte[]> Handle(byte[] package);
    }
}