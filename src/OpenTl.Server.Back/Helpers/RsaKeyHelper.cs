using System;
using System.Security.Cryptography;

namespace OpenTl.Server.Back.Helpers
{
    using OpenTl.Common.Crypto;

    public static class RsaKeyHelper
    {
        #region Keys

        public static string PublicKey =
@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7Kh5FpK5KxFNFUSQ8yWK
GzBW4OJX+ju3S2zX179uJFqisgaC256UgI4UsUfZhR38zi6g4AZqlOUOZcLNwp3r
6zK0ujxmZNu1M3LQLcS1D4aoWHZRHnEz0wuZqhOnIOXA31ATB8kCqiIu0FbKrZ/5
HnPfdDFXt6bfv3+vgyyJwI/G8umiLHo0DSBuXnxo4v6/9hEQcj8acZ26sNj3u9N8
M1WncvMct9os0FJWKaeJ0BUatxHrZC/xsaW5nS9f6Pjw9TfMwuU9qnEZye4Gmgu8
6lv1fbUcg0zl0S4FQpDnf0aKOaU2+DiNxM6DqCBJs7Fz9OZ+LB7bw+5006/defll
cQIDAQAB
-----END PUBLIC KEY-----";

        public static string PrivateKey =
@"-----BEGIN RSA PRIVATE KEY-----
MIIEpQIBAAKCAQEA7Kh5FpK5KxFNFUSQ8yWKGzBW4OJX+ju3S2zX179uJFqisgaC
256UgI4UsUfZhR38zi6g4AZqlOUOZcLNwp3r6zK0ujxmZNu1M3LQLcS1D4aoWHZR
HnEz0wuZqhOnIOXA31ATB8kCqiIu0FbKrZ/5HnPfdDFXt6bfv3+vgyyJwI/G8umi
LHo0DSBuXnxo4v6/9hEQcj8acZ26sNj3u9N8M1WncvMct9os0FJWKaeJ0BUatxHr
ZC/xsaW5nS9f6Pjw9TfMwuU9qnEZye4Gmgu86lv1fbUcg0zl0S4FQpDnf0aKOaU2
+DiNxM6DqCBJs7Fz9OZ+LB7bw+5006/defllcQIDAQABAoIBAQDbvm95E1IWeGEf
z2PcMc40AsWY2PKh6pL+2RjuPtURsosBTORy8qOnXsY9+p4yaa7U8Bz3B14t1SZy
PNj7zdFCuflwOCdHnW56UDCXXuBUg0+LnIkkAC8D1vCfKNJ1zIAzmtGg1/e+bDEV
yJE3eKRDr4ocBLkTUULPOMuKvG9IOS9/Gss77XlPhfGKmvhgJOcMo7qa5HQMG1f2
CYg28d3/87WVM226Bw8Haw2yRpU4BhwpoyQC4+Oqb5DMDy5nW3dOpJKWkOvEgnid
TlD1GgbhQTTiE0euPwfurv6Gq6j0Hw1Mo0DcWZTOkVA4vnWcstryXyL0uwJgJnow
bqM6IDQFAoGBAPg/CsSMapLuYVnQiEIbZcyqJXtNUTV9JfhZf7mULf4COg9t2En/
/Slyr/XQmVm7kAjRYUoJQJj2hufLlRYKeGp/dtOmOyWKWujMkkZWFabFcx/MMCxV
2CU5qC7NdXQuM0+YhGL8+wN/Fd/s+6rjY27tGyY8IhcPRw2wfYDaTLPTAoGBAPQM
xdzpR0j9mXh/rpDfOemTkXVA7yCRaEGveMjmN4JQuCdvtN/YjTEkjiY4Zo4Vhhu0
WK/4PyTo07Jdm2fXO6+Dpkm7THpD7ZlO4AA+7yQcGZiKlYkIFWLiovcTG4+46s20
60988LFZZ5Y/b/9TqGOAv+QcTXWu8Q4d1FpBmmsrAoGAH/wfawucf6nvKR9RLxNQ
nnodsjFYEUg6qDD/3+1Tr7KhwHMqtv4gpEJ+oXFrEMC14iz4GA3xIMRCxYLZhql6
sl4R9Vspq8CTzgLtpdpZl89A8gvg+RVmcAVpwf3+8CUzv0GrbQWjYePx8ZQbKP17
RkOOh/KfgdEaGo1u3jdCQrkCgYEAwlcZiR1K4tjvyYEDpeHc4B/fCRw4UBr6hrQQ
3wpU0bUtsFnIEykC5ktR1yW6pRKGxLEMnrR0tBOj4Lmh80L2CIIxfS1lbUeCgT7K
KefzzzGBQHO7OG/zd9c6Jr5UiFKcyEp1x8qacN/dGUxTB2O7B+GS7TeMh1ZUJwWi
AG2VZhkCgYEA6yU0XJUup+flYmua/s2ubl/iA+BCSTCota9fcgPcc0KKzCmDJZae
Q9otjKS6IJcCIYXN8kQSGC0CPqROKF9vEA4yPeKdu5c9pkPeN71F7BbIh8U8ja2A
hm41CyH57XLQ8PHFn7UVUqNZzj0MQKy0YnRONkFdj3McSgum6Es7RnM=
-----END RSA PRIVATE KEY-----";

        #endregion

        private static long publicKeyFingerPrint;
        public static long PublicKeyFingerprint { get; } = publicKeyFingerPrint != 0
                                                                        ? publicKeyFingerPrint
                                                                        : (publicKeyFingerPrint = RSAHelper.GetFingerprint(PublicKey));
    }
    
    
}