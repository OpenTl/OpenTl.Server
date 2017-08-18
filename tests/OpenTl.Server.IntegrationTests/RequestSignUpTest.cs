namespace OpenTl.Server.IntegrationTests
{
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;

    using Xunit;

    public class RequestSignUpTest: BaseTestWithEncryption
    {
        [Fact]
        public void SimpleTest()
        {
            var requestSendCode = new RequestSendCode
                          {
                              PhoneNumber = "7777",
                              ApiHash = "aaa",
                              ApiId = 1111,
                              AllowFlashcall = false,
                              CurrentNumber = true,
                          };

            var responseSendCode = SendEndRecieve(requestSendCode).Cast<TSentCode>();

            var request = new RequestSignUp
                          {
                              FirstName = "aaa",
                              LastName = "bbbb",
                              PhoneCode = "7777",
                              PhoneCodeHashAsBinary = responseSendCode.PhoneCodeHashAsBinary,
                              PhoneNumber = requestSendCode.PhoneNumber,
                          };

            var response = SendEndRecieve(request).Cast<TAuthorization>();

            var responseUser = response.User.Cast<TUser>(); 
            
            Assert.Equal(responseUser.Phone, request.PhoneNumber);
            Assert.Equal(responseUser.FirstName, request.FirstName);
            Assert.Equal(responseUser.LastName, request.LastName);
        }
    }
}