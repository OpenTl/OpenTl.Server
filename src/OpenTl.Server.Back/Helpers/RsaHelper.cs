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
                if (_publicKeyFingerPrint == 0)
                {
                    using (var sha1 = SHA1.Create())
                    {
                        var publicKey = Convert.FromBase64String(PublicKey);
                        var hash = sha1.ComputeHash(publicKey);
                        
                        _publicKeyFingerPrint = BitConverter.ToInt64(hash, hash.Length - 8);
                    }                     
                }
                
                return _publicKeyFingerPrint;
            }
        }

        #region Keys

        public static string PublicKey =
            @"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAgxRVtopq6e006lp4zeOZ
zRGejcrzNTgqC6eJPnXfdym+MimeTYaEFjVCPuig6JSeVc67GcSIQzLIPeWmA2sQ
BWtYknGPobnpCtv8zMAv0SoC0Kn/caNjLCdFpk3B4U5rXQ1/1oXyuydYm8DU+Oe+
zePqoXpX00rtoGf1aAz8zgNko4YLz8ml9KIzSPyAd/IR9zgjTMPGBtyV+3e4k+C7
XjnHiN1ZuKlGq/WEEX2V24UmDC/0W3dB1Vi2pb8zJMrJS+LsKqtyAM11pyhGkBeD
gb5BmPlj64LPxQc6qTlnSo5Sd1Gfu7I0g7THX0Tcn6NOyHBBIMXQQHaUcdtMNczk
TQIDAQAB";

        public static string PrivateKey =
            @"MIIEogIBAAKCAQEAgxRVtopq6e006lp4zeOZzRGejcrzNTgqC6eJPnXfdym+Mime
TYaEFjVCPuig6JSeVc67GcSIQzLIPeWmA2sQBWtYknGPobnpCtv8zMAv0SoC0Kn/
caNjLCdFpk3B4U5rXQ1/1oXyuydYm8DU+Oe+zePqoXpX00rtoGf1aAz8zgNko4YL
z8ml9KIzSPyAd/IR9zgjTMPGBtyV+3e4k+C7XjnHiN1ZuKlGq/WEEX2V24UmDC/0
W3dB1Vi2pb8zJMrJS+LsKqtyAM11pyhGkBeDgb5BmPlj64LPxQc6qTlnSo5Sd1Gf
u7I0g7THX0Tcn6NOyHBBIMXQQHaUcdtMNczkTQIDAQABAoIBAHOa052Spoh5tFmc
QT8UpOi/yV60x5sAVdTBhcGYo+Ws4xnKqtsk8AnGHw7sjD2UFbEICuvG8YaSmxg8
GhaZrh+ZdRxzG4I/PNFfA65xMbr/mkv+IlRPDYA7gqoRmhTj3LzM0xqYOGPnZ7a2
cx6zBQ0BHkYcaKjpRHpcNYv3KF9oPfsFmDdNTjKC98pi0/cbmNh5MNvwj43PL29y
MQBWouWYZU8TLKiEt/sGQXdkeEQCfAi/QQWc79R+tu1Ykb1Y9kPqV6AgBRiTPTPd
955Q0m/oanrgA/74zTRSKbyIOvKn+CCJJxG5XWMmVJQPPltgLKYsEJ41pozAKKpV
bRbiMAECgYEA9JVlLPtikbC4yiAi9DdDWh0vpUl2v3XtJAHPOLC8XHENLAQHguD1
yo6BlwW5yOlWPjDQozIV0s4w10h+jKbt+gtD+VDOmYR8ZZT0v/f/kL3WTFq2tFe7
8V6IIZdDYA9ZX3LClPdAHfZ+Ch2y1Pi2I/b7OhuVqIarKWN36wS8pIECgYEAiTKk
vGcs1tN6rIdNItUTauje0VPVLWhMwFKWLdB4gjvkB2C6OIOB4Igu9FHuLew358WR
F4bcUzUDCv/IpPou7Rm2mhTlFAuIj1lDzy0n/vNokUYCWGrv87VECl8WxSwZgx6h
pVjDqF80uIh0OG5Y4cyQdSJtd8HX7/PbkcAkqc0CgYBeOLtMU9+KHplhjHXKvQte
SMYVF7L+WRCtAWFyBmvZ9NdNMJQwMDef+7wikNsccf9+X9HQPLg5iKM6HDxcNOaS
oApknmOosmg1ved3mLNEcE2BBqVB3laRyogI4Lvc4qzcX1pkhseVg2LUxNIix1lr
i3cG0J+b151SiXYl/KIrgQKBgEtCWpd2yWP2kf5+IdQPqh0lLMULBY9o/WEqbHMp
PlHCZK2fY8eIbAAs9ATVxJ+wSmJ7P8H2GnoSRF8OPJQzIPay+jW/bIH4aaqovsew
75WtFBlMnBDAaGv5bR97VdRHAp+od+dpr7p2r0bio04pSdxjCIMYpY/h54Aa9sEW
84WpAoGAdoPVTyEFDpfOTxOGrfzZVmDPkNzvpPbeEIrsFJ1lFV6+UZ0DIPfr7Uxs
JQHlMlZAgJa8XuvYUuxatCXRz3ofr1z4395g15CF/hC08T8VJoVUOXkVTmW71BJ3
Ef9nHLMWivDk2OldWnVqJIsRod2gGwnRkZnDwRdytJqDkE4/ovA=";

        #endregion
    }
}