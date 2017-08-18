namespace OpenTl.Server.IntegrationTests
{
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;

    using Xunit;

    public class RequestSendCodeTest: BaseTestWithEncryption
    {
        [Fact]
        public void SimpleTest()
        {
            var request = new RequestSendCode
                          {
                              PhoneNumber = "7777",
                              ApiHash = "aaa",
                              ApiId = 1111,
                              AllowFlashcall = false,
                              CurrentNumber = true,
                          };

            var response = SendEndRecieve(request).Cast<TSentCode>();
            
            Assert.NotEmpty(response.PhoneCodeHash);
            Assert.False(response.PhoneRegistered);
        }
    }
}