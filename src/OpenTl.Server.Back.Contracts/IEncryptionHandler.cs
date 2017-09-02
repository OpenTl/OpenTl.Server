namespace OpenTl.Server.Back.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IEncryptionHandler : Orleans.IGrainWithIntegerKey
    {
        Task<byte[]> TryEncrypt(byte[] package, ulong authKeyId);
        
        Task<byte[]> TryDecrypt(byte[] package, ulong authKeyId);
    }
}