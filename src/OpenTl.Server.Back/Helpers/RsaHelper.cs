using System;
using System.Security.Cryptography;

namespace OpenTl.Server.Back.Helpers
{
    public static class RsaHelper
    {
        private static long _publicKeyFingerPrint;

        public static long PublicKeyFingerprint
        {
            get
            {
                return 1u;
//                if (_publicKeyFingerPrint == 0)
//                {
//                    using (var sha1 = SHA1.Create())
//                    {
//                        var publicKey = Convert.FromBase64String(PublicKey);
//                        var hash = sha1.ComputeHash(publicKey);
//                        
//                        _publicKeyFingerPrint = BitConverter.ToInt64(hash, hash.Length - 8);
//                    }                     
//                }
//                
//                return _publicKeyFingerPrint;
            }
        }

        #region Keys

        public static string PublicKey =
@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA8k1LTajz2N9RMjDnv1Jq
LUmE+MZWCOTMC0FqjZmjNWm9zhgy7Rv12nUz3I2rLiKEZp/O42ThfTtDgRVrFkkE
ALl0YWrWwt5QQq5k51POngCt9n6bLi2Q82HYMotvIhN6w15B692Urbu6RtDRnfdb
ojRDWGh/DNElRIuy+T1bWCIISCTX47PrXJs4I1WPw/xYDl9zA9xhEMIQx6NwoRlj
XBdWrYayFbllXqV3EPhhIpuQ6cqNsudbLGCFdsaRuNuTJzo43hf549xrumH0I/4K
1FLCbxCWz3gnWLaxoN+RtSJvXvyEfMFRdsWSW/mMr3WFwb8nakabgQ1YbtT5576M
OQIDAQAB
-----END PUBLIC KEY-----";

        public static string PrivateKey =
@"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA8k1LTajz2N9RMjDnv1JqLUmE+MZWCOTMC0FqjZmjNWm9zhgy
7Rv12nUz3I2rLiKEZp/O42ThfTtDgRVrFkkEALl0YWrWwt5QQq5k51POngCt9n6b
Li2Q82HYMotvIhN6w15B692Urbu6RtDRnfdbojRDWGh/DNElRIuy+T1bWCIISCTX
47PrXJs4I1WPw/xYDl9zA9xhEMIQx6NwoRljXBdWrYayFbllXqV3EPhhIpuQ6cqN
sudbLGCFdsaRuNuTJzo43hf549xrumH0I/4K1FLCbxCWz3gnWLaxoN+RtSJvXvyE
fMFRdsWSW/mMr3WFwb8nakabgQ1YbtT5576MOQIDAQABAoIBABzXeIQ4/TWud2LL
EXrjm4Hig3J9YVZTrboVQlKynAvKl25F0SIKNvyXAOJa9qpaL3prwVut8W1PtZxS
6VlQvao8aQ8DgabWgaU+TwJ+JlUGba6uqVgY0m02E18I2+Spfu5sdNpXmNAJTYYK
azkrbXvkTrPiVGU5K/95xYcHd60IvLcVYTk9KwDZymO3fY6u9QOqrGPMYzC1P2My
U7+ZFA0PJPBLEQWj2AIXBTYWsoj75P11r4o0SAQYsY7dT+jHbykyoYCxiB3ENdAq
jNshgjuOOKLcPIXf/S1ztDGk3i/0yiWDFtmCzw/HtRf8PRLMJTJzhxgE11N8NWOG
Ezl4bYECgYEA/p6MYVrIOZ7fxas/4s3BhJtkt69aqsll82+cxa/SLkH1mabr3+q0
qnAxkeVdZKuR1LCFdcy/Lxd0YZF8lwAxXQInqCFs7aLSnkz/qP1up7b7w/8EfKja
awjovzf4bYLnBxMJnrQE1NiywX9/PR4bp+wQDj+M+DGN+lRM2EBbADECgYEA852l
tfLOWTwluzTISSloBiE+h0SZHETJKPqj5HjQr7tq4tDZ+2UblPcVHKBWBMcpTi3T
4IqjW30oEvoZSZNwnvB5jYKl1hjtEbSmKC3M8EnKgEGdy0G9CmZrNtppl3xVADGG
Ieqt9pOMCx7J42/WomKB/aYd2PzABRTwEaeIEokCgYBzrfbbeFJFk3/ZH7+rvI1y
QONWbM3FkDDIk+nnCsV0DLWXtHWvysOAN+7deRagWS6tMfHAnmAx9fcDKQUw2X6T
4hnAUkdaA8Kq9xKkZVfzzLe/yUnxlQl+3ZJY5gXxQyrRVP3m46TaSwWT0eguDVLF
TQPSZV8Xl/QISmqLSVnO0QKBgHc71Xs0F6K9OYpizxRf27YEV2JFRNr9H6ea5NRR
/XHFPQ8+QTI1zkYemIqmPvOftqu46lagBEwm+ZIwLmhAbYKdGCEWrKwZDw73Z8uK
fx+sPhyAAQcWabvJXPg/9iZaiA/MLWY0QmjI1mYq740Nk/NuW0kWIM2vBxx1nvpF
EOhhAoGBAOwLFonnMLAME+wScAGFIFgwRysPhAcQRrfI4uuwf8KMWSwwOnjnB8mT
T+Yjt4ln9yYg5aOCbSOSztkB0bbgchkMAWYyOyGHZNyNnuYik12+nffH3ZgYsOhS
gf9PH7yar3Sp8CXLPnZD4sxgHywRUFVDrCu99qRHis3yK1EF7ilX
-----END RSA PRIVATE KEY-----";

        #endregion
        
    }
    
    
}