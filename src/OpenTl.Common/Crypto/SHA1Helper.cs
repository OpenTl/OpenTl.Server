namespace OpenTl.Common.Crypto
{
    using System.Security.Cryptography;

    public static class SHA1Helper
    {
        public static byte[] ComputeHashsum (byte[] data)
        {
            using (var sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(data, 0, data.Length);
            }
        }
    }
}