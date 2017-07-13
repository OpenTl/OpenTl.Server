using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OpenTl.Common.Auth;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using Xunit;

namespace OpenTl.Common.UnitTests
{
    public class ReqPqTest
    {
        [Fact]
        public void SimpleTest_Not_Throws()
        {
            ReqPqHelper.Client(out var nonce);
            var publicKeyFingerPrint = 12343211L;
            var resPq = ReqPqHelper.Server(nonce, publicKeyFingerPrint, out var p, out var q, out var serverNonce);
            
            Assert.Equal(16, resPq.ServerNonce.Length);
            Assert.NotEmpty(resPq.Pq);
            Assert.Equal(new List<long> {publicKeyFingerPrint}, resPq.ServerPublicKeyFingerprints.Items);
        }
    }
}