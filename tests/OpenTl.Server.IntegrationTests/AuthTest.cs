using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.Crypto;
    using OpenTl.Server.IntegrationTests.Helpers;

    public class AuthTest
    {
        private static int mesSeqNumber;
        
        [Fact]
        public async Task Step1Test()
        {
            var networkStream = await NetworkHelper.GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);

            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, mesSeqNumber);
            mesSeqNumber++;
            
            Assert.Equal(nonce, resPq.Nonce);
            Assert.Equal(16, resPq.ServerNonce.Length);
            Assert.NotEmpty(resPq.Pq);
            Assert.Equal(new List<long> {RSAHelper.GetFingerprint(AuthHelper.PublicKey)}, resPq.ServerPublicKeyFingerprints.Items);
        }
        
        [Fact]
        public async Task Step2Test()
        {
            var networkStream = await NetworkHelper.GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            
            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, mesSeqNumber);
            mesSeqNumber++;

            var response =  AuthHelper.GetStep2Response(resPq, networkStream, mesSeqNumber, out var newNonce);
            mesSeqNumber++;

            Step3ClientHelper.GetRequest(response, newNonce, out var clientKeyPair, out var serverPublicKey);
            
        }
        
        [Fact]
        public async Task Step3Test()
        {
            var networkStream = await NetworkHelper.GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            
            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, mesSeqNumber);
            mesSeqNumber++;

            var serverDhParams =  AuthHelper.GetStep2Response(resPq, networkStream, mesSeqNumber, out var newNonce);
            mesSeqNumber++;

            var response = AuthHelper.GetStep3Response(networkStream, serverDhParams, mesSeqNumber, newNonce, out var clientAgree, out var serverTime);
        }
    }
}
