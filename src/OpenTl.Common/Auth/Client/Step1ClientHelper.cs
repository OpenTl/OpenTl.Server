namespace OpenTl.Common.Auth.Client
{
    using System;

    using OpenTl.Schema;

    public static class Step1ClientHelper
    {
        private static readonly Random Random = new Random();

        public static RequestReqPq GetRequest(out byte[] nonce)
        {
            nonce = new byte[16];
            Random.NextBytes(nonce);
            
           return new RequestReqPq {Nonce = nonce};
        }
    }
}