using OpenTl.Schema;

using Xunit;

namespace OpenTl.Common.UnitTests
{
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.Auth.Server;

    using Org.BouncyCastle.Security;

    public class AuthTest
    {
        private const string PrivateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIIEogIBAAKCAQB0Qo6bdREPSIhI7olgf7qIJALG/6C6QafYMWrstR4/bT6/klyw
wYawaM4Jk4U0tF0BzoaSs2Scgu+fVoDUSin5fL+LDehV1FuK4AnXrWVfk+URtezm
5v3TEG6rELt3isv3U2s3eq3gVbg2Zc8rhZAOcOexRsI7HLnP0GgOze+jBafI1dht
Q1aR82U553jXAnvqhB7M7fynVeWWey/ZCvVkIh91O11RkIAHc0ZrENVycXoH22A6
qoNZ2DV5Kozx+mA0dZOyvc/wJsUCGV+2dKCA2sTBbSi6c5NrKUR2HmELZ4z4k3HI
1JwDaS1Wutshn29XST25+LNoUv+ASPm3xv3FAgMBAAECggEAD8z36sLnVadq9laN
DzNs25HhWsBlTMsdj7FvscoP2Vj0nT68ID29G36y+fQ1t2sH9PXnrkp5UxgsjwBH
tBJ8T+8XbUOvnvKkmmRGkix7yZilGloaX2tndecW02MLx3xyqiOJP9oJhaLAzYva
szSM1pmbs6lFPplx23+clN6CQiqUrGh5L9iHvCq76M7TwPc8r/OSjeZ+X+HMVQ5Q
qNvqDscgZq26CfccJfclpCkWHfNxl10tSNoIUTA6WjasLOr9iT4Y220u6Ixn8vjH
lotiXGPLBLgR8pTKFzysSAoa+5MYvPitOr8lgllBRWOZVsKq6LpOEHGeZHJRkBtd
uCMrhQKBgQCzSPvOooIkD8njqJj57xOVMBKym3Wn5ssiooanxoBy4hciKIv4DmaY
qO8t2R4duSO+UMVoc6/ZUE+9zmG2IBMZgYZoo7Tjpj1jWmTVkc5mSb96ZapBQsJm
HBOoCaF/Uxcq9JZaBfxR9T7FtQJWlvLNvLMPlm//WqMhnxfpyV9KGwKBgQCmAcJA
gaxfLmzCRxqSkjKgRxFEmknn3mZA9ak8tRYNR862qKAVoPFWXdFV5T+miQ80fgMQ
7NnsQExwlYPgkzas+6kfWI3mZ4j354w6YFiZeIvjP4jigVtV8tTIwFge63FXHuDy
/RfM7ftlDiMgp6VBFCginubAAg4UtMJppcdVnwKBgF1Hkcv2/WMp287ZP456d2Pd
uTy4acTL7dC9YvYT72zWeVbKZdjQFU1ER/1Aw8yPMSup9qvb1RhqErMcl/YcRhO1
MmtrEYiIS9RPibscu5i4silKkASYaXCbFxcMu7v1TU6KJ1f7WUaDJP/Up7zJmNa2
YSuxB+CCFfI+RkDbBvsVAoGBAKO+Cl/cSCyeOzKqNeCrCfqj/rlfQlTOPqgGaEb0
C2tQD+qFi1mqJUJeSbRi6pSWjoPlY5rXNODeT0ehgrIKIx0fIiNEErtYFncIUn3m
OJ4wlDKzoY5hr7ioTBA0APeGnoYjdBjZYheGCbkU57s3AT6e5jO37r0nicIMxdK1
bVfbAoGARDs3I2oLqvIr0qg5F71i2+5QQjiXyRgWxMnwfXGX2MR2CkxM+hvTl4OY
lAkq2U7zExdhkaoUB4SJifd0C95GGHBpTEPHRmTXCSein1GQj0PmJhIwGz9YJeC1
oWerjbmbcOxXw+TfFcTqwOPxJ5oqeJ5cP6DNMAJ7aRj4wHtageo=
-----END RSA PRIVATE KEY-----";

        private const string PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBITANBgkqhkiG9w0BAQEFAAOCAQ4AMIIBCQKCAQB0Qo6bdREPSIhI7olgf7qI
JALG/6C6QafYMWrstR4/bT6/klywwYawaM4Jk4U0tF0BzoaSs2Scgu+fVoDUSin5
fL+LDehV1FuK4AnXrWVfk+URtezm5v3TEG6rELt3isv3U2s3eq3gVbg2Zc8rhZAO
cOexRsI7HLnP0GgOze+jBafI1dhtQ1aR82U553jXAnvqhB7M7fynVeWWey/ZCvVk
Ih91O11RkIAHc0ZrENVycXoH22A6qoNZ2DV5Kozx+mA0dZOyvc/wJsUCGV+2dKCA
2sTBbSi6c5NrKUR2HmELZ4z4k3HI1JwDaS1Wutshn29XST25+LNoUv+ASPm3xv3F
AgMBAAE=
-----END PUBLIC KEY-----";
        
        [Fact]
        public void SimpleTest()
        {
            Step1ClientHelper.GetRequest(out var nonce);
            var publicKeyFingerPrint = 12343211L;
            var resPq = Step1ServerHelper.GetResponse(nonce, publicKeyFingerPrint, out var p, out var q, out var serverNonce);

            var reqDhParams = Step2ClientHelper.GetRequest(resPq, PublicKey, out var newNonceFromClient);
            var serverDhParams = Step2ServerHelper.GetResponse(reqDhParams, PrivateKey, out var parameters, out var newNonceFromServer);
            
            Assert.Equal(newNonceFromClient, newNonceFromServer);
            
            var setClientDhParams =  Step3ClientHelper.GetRequest((TServerDHParamsOk)serverDhParams, newNonceFromClient, out var clientKeyPair, out var serverPublicKey );
            var setClientDhParamsAnswer = Step3ServerHelper.GetResponse(setClientDhParams, newNonceFromClient, parameters, out var serverAgree, out var serverSalt);
            
            var clientKeyAgree = AgreementUtilities.GetBasicAgreement("DH");
            clientKeyAgree.Init(clientKeyPair.Private);
            var clientAgree = clientKeyAgree.CalculateAgreement(serverPublicKey);
            
            Assert.Equal(serverAgree, clientAgree);
        }
    }
}