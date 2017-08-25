
namespace OpenTl.Server.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Requests;
    using OpenTl.Server.UnitTests.Builders;

    public class RequestSendCodeTest
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

            var userRepo = RepositoryBuilder.Build<User>().Object; 
                
            var grain = new RequestSendCodeHandlerGrain(userRepo);
            
            var responseData = await grain.Handle(1, requestData);
            
            var response = Serializer.DeserializeObject(responseData).Cast<TSentCode>();
            
            Assert.NotEmpty(response.PhoneCodeHash);
        }
    }
}