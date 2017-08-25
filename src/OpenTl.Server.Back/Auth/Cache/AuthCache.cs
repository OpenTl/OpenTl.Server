using System;
using System.Runtime.Caching;
using Org.BouncyCastle.Math;

namespace OpenTl.Server.Back.Cache
{
    using Org.BouncyCastle.Crypto;

    public class AuthCache
    {
        private static readonly MemoryCache Cache  = new MemoryCache("auth");

        public byte[] Nonce { get; set; }
        
        public byte[] ServerNonce { get; set; }
        
        public byte[] NewNonse { get; set; }
        
        public BigInteger P { get; set; }
        
        public BigInteger Q { get; set; }

        public AsymmetricCipherKeyPair KeyPair { get; set; }

        private AuthCache()
        {
        }
        
        public static AuthCache GetCache(ulong clientId)
        {
            return (AuthCache) Cache.Get(clientId.ToString());
        }
        
        public static AuthCache NewAuthCache(ulong clientId)
        {
            var cacheItem = new AuthCache();
            Cache.Add(clientId.ToString(), cacheItem, new CacheItemPolicy {AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)});

            return cacheItem;
        }
    }
}