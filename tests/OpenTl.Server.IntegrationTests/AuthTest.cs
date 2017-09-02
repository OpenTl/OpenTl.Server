using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.Crypto;
    using OpenTl.Server.IntegrationTests.Entities;
    using OpenTl.Server.IntegrationTests.Helpers;

    public class AuthTest
    {
        [Fact]
        public async Task Step1Test()
        {
            var networkStream = await NetworkHelper.GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);

            var session = new TestSession();

            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, session);
            session.SeqNumber++;
            
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
            var session = new TestSession();

            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, session);
            session.SeqNumber++;

            var response =  AuthHelper.GetStep2Response(resPq, networkStream, session, out var newNonce);
            session.SeqNumber++;

            Step3ClientHelper.GetRequest(response, newNonce, out var clientKeyPair, out var serverPublicKey);
            
        }
        
        [Fact]
        public async Task Step3Test()
        {
            var networkStream = await NetworkHelper.GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            var session = new TestSession();

            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, session);
            session.SeqNumber++;

            var serverDhParams =  AuthHelper.GetStep2Response(resPq, networkStream, session, out var newNonce);
            session.SeqNumber++;

            var response = AuthHelper.GetStep3Response(networkStream, serverDhParams, session, newNonce, out var clientAgree, out var serverTime);
        }
    }
}
