using OpenTl.Schema;

using Xunit;

namespace OpenTl.Common.UnitTests
{
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.Auth.Server;

    using Org.BouncyCastle.Security;

    public class AuthTest
    {
        private const string PrivateKey = @"-----BEGIN PRIVATE KEY-----
MIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQD2XnScITdsNnhO
onv1ZxN/r/HHtp7uLzXh4DmTQXb3lMatcy8pOrril6zeUpUvZZ5mwRFMMiKTMg8S
GMkycJY1sB8ItH+09YCr4nnGNGoTCSyYgG8i/zMyFo55wXD+ct6B//bX5gkngapT
G+K4XCznBEZ3CmKbcpoLkKgMJi9IlUG7BzcIYJZcBCsyzllMHIAv3D3cUBhZ+3KQ
4OyciNfvNkVf3qacWqdeGj9cjvWbICO/9p/OzIY3Iw5u8ihldmlwn0JKLOLjkDgn
IKBI/ZFoJn/ve5pt8PCArG63oqIiq0tcpO3bqkseb+M9rJN/iRu5s7szGBrc7wM0
wwsh6LYRAgMBAAECggEBAM2HVP6fE05aTplDIehC2lhuXmS+q+2GQwwxc7auKWI3
ols2uURK83dvE07RVBWbT77nF9gaBxAG9d2iAc7rb9RQHkOz2MoM+Q/ruq0oMsm4
oJSEqLJodbw2ZxuZzfrnt0p6T+IizMZBfOciX0xv0SHURjiu2D5YJ7y4LAkendmM
z6XbIch9JjOWujOj2PpRjepJeyCVgZAp12xXf1gJTakaEq+uqbH1Yr62QVgPz1rP
eg0HSsycupMwuqhp96ObETTtZUsomz9bRizjT7vstcpvxUyKWb2op9f/8Q+bOv0q
9yAbQCSrmkTDF8ZPp6R0WIQTaWmoVbDqiN3eZDvNSAECgYEA/PI7/EcsM2BXstLX
ETJrqyMKZ7VbzlbjqbJsoYzrccpk9uKsliVrbdAKpQ5RomRHzD8v6YOdOXbDr0sy
vy7CErj00zx+9Y+w2cnMEzGCnJ3lNnOQZfh+ZXnzj6PfTysvznV7Fd0SDrTg+E2H
alZbP4jAg80Aa7mH1Wz+fm5X8IECgYEA+Vfkq6wT+gTx9mPhU+Yw/1LJZGqvyP7G
G2i/tYuXjCTnM9G/mNOmZXrbxe9BtLBwItCBmdR13zBQGTGKAOMyDC7rKEk0/3Zx
340BCIEUfP2V21kmJm9bxTFEbfi9VSDMyWmtsGtbuJxiRMgh1iV3UBU7DlHmCQGR
K+VpJiRq/ZECgYEAwYEukpiAH+2cKZlSHj3MUGfGFgCP8xJVqARvRkdf9vU2uAhA
r7yAqsl020BRA0JkCsRsCuA87zEEEp4cfFVw8pG7+b1WAtWiHXpbPHOrz9Sa/UT7
OocXatoYkdJzi0UmlXMabmdo4QdO86Wb6qWVuM0NOccig0azu3peVqpjbAECgYEA
8+7j3HtNU9TQrn6Ka50QpEjmk6G39YgiYaA5pS86hLV5pv11VtsAt9jY9yyZeF8A
ZV7c25S5+C54vhzz6OhcBwVDs7bi7WWAA/cbYql9VMZ1fBEcPI/HFQSc8IVfHNAo
6IbFkImHQvVjQe0VBT7EGgmbK+g9huQMlQgrtU/9h7ECgYB9o8F7vLLvSuYwP+ub
59WaLTOoEc97Fd7Fcw3eeUarZEahg+A7b9D4T5f+w/zVWIP4nZE3sYmqyXOoagz3
YpdJbJtzAARUjGUHpFtezXEfm4O5B17I/IfnKQXJhmjabGF7NBE7fbsxlFO3EEl6
JT6AWalmxkajT3OPJZCgecHP0Q==
-----END PRIVATE KEY-----";

        private const string PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA9l50nCE3bDZ4TqJ79WcT
f6/xx7ae7i814eA5k0F295TGrXMvKTq64pes3lKVL2WeZsERTDIikzIPEhjJMnCW
NbAfCLR/tPWAq+J5xjRqEwksmIBvIv8zMhaOecFw/nLegf/21+YJJ4GqUxviuFws
5wRGdwpim3KaC5CoDCYvSJVBuwc3CGCWXAQrMs5ZTByAL9w93FAYWftykODsnIjX
7zZFX96mnFqnXho/XI71myAjv/afzsyGNyMObvIoZXZpcJ9CSizi45A4JyCgSP2R
aCZ/73uabfDwgKxut6KiIqtLXKTt26pLHm/jPayTf4kbubO7Mxga3O8DNMMLIei2
EQIDAQAB
-----END PUBLIC KEY-----";
        
        [Fact]
        public void SimpleTest()
        {
            Step1ClientHelper.GetRequest(out var nonce);
            var publicKeyFingerPrint = 12343211L;
            var resPq = Step1ServerHelper.GetResponse(nonce, publicKeyFingerPrint, out var p, out var q, out var serverNonce);

            var reqDhParams = Step2ClientHelper.GetRequest(resPq, PublicKey, out var newNonce);
            var serverDhParams = Step2ServerHelper.GetResponse(reqDhParams, PrivateKey, out var parameters);
            
            var setClientDhParams =  Step3ClientHelper.GetRequest((TServerDHParamsOk)serverDhParams, newNonce, out var clientKeyPair, out var serverPublicKey );
            
            var setClientDhParamsAnswer = Step3ServerHelper.GetResponse(setClientDhParams, newNonce, parameters, out var serverAgree, out var serverSalt);
            
            var clientKeyAgree = AgreementUtilities.GetBasicAgreement("DH");
            clientKeyAgree.Init(clientKeyPair.Private);
            var clientAgree = clientKeyAgree.CalculateAgreement(serverPublicKey);
            
            Assert.Equal(serverAgree, clientAgree);
        }
    }
}