namespace OpenTl.Server.Back.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IEncryptionHandler : Orleans.IGrainWithIntegerKey
    {
        Task<byte[]> TryEncrypt(byte[] package, ulong authKeyId);
        
        Task<Tuple<byte[], ulong>> TryDecrypt(byte[] package);
    }
}