
namespace OpenTl.Server.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Requests;
    using OpenTl.Server.UnitTests.Builders;

    public class RequestSendCodeTest: BaseTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var request = new RequestSendCode
                          {
                              PhoneNumber = "7777",
                              ApiHash = "aaa",
                              ApiId = 1111,
                              AllowFlashcall = false,
                              CurrentNumber = true,
                          };
            
            var requestData = Serializer.SerializeObject(request);

            this.BuildRepository<User>(); 
            
            RegisterSingleton<RequestSendCodeHandlerGrain>();

            var grain = Resolve<RequestSendCodeHandlerGrain>();
            
            var responseData = await grain.Handle(Guid.NewGuid(), requestData);
            
            var response = Serializer.DeserializeObject(responseData).Cast<TSentCode>();
            
            Assert.NotEmpty(response.PhoneCodeHash);
        }
    }
}