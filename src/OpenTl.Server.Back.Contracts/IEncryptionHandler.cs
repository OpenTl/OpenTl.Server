namespace OpenTl.Server.Back.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IEncryptionHandler : Orleans.IGrainWithIntegerKey
    {
        Task<byte[]> TryEncrypt(byte[] package, Guid authKeyId);
        
        Task<byte[]> TryDecrypt(byte[] package, Guid authKeyId);
    }
}