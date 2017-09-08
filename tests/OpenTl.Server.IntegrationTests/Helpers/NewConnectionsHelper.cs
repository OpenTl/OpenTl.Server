namespace OpenTl.Server.IntegrationTests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Schema.Help;
    using OpenTl.Server.IntegrationTests.Entities;

    public static class ConnectionsHelper
    {
        private static readonly Random Random = new Random();
        
        public static IReadOnlyList<ConnectionInfo> CreateConnections(int numberOfConnections)
        {
            return Enumerable.Range(0, numberOfConnections).Select(i => CreateConnection()).ToArray();
        }

        public static ConnectionInfo CreateConnection()
        {
            AuthHelper.Handshake(out var stream, out var clientSession, out var time);

            var requestInvokeWithLayer = new RequestInvokeWithLayer
                          {
                              Layer = SchemaInfo.SchemaVersion,
                              Query = new RequestInitConnection
                                      {
                                          ApiId = 0,
                                          AppVersion = "1.0.0",
                                          DeviceModel = "PC",
                                          LangCode = "en",
                                          LangPack = "tdesktop",
                                          SystemLangCode = "en",
                                          Query = new RequestGetConfig(),
                                          SystemVersion = "Win 10.0"
                                      }
                          };
            stream.EncryptionSendAndRecieve(requestInvokeWithLayer, clientSession).Cast<IConfig>();

            var requestSendCode = new RequestSendCode
                                  {
                                      PhoneNumber = Random.Next(10000000).ToString(),
                                      ApiHash = "aaa",
                                      ApiId = 1111,
                                      AllowFlashcall = false,
                                      CurrentNumber = true,
                                  };

            var responseSendCode = stream.EncryptionSendAndRecieve(requestSendCode, clientSession).Cast<TSentCode>();

            var requestSignUp = new RequestSignUp
                          {
                              FirstName = "aaa",
                              LastName = "bbbb",
                              PhoneCode = "7777",
                              PhoneCodeHashAsBinary = responseSendCode.PhoneCodeHashAsBinary,
                              PhoneNumber = requestSendCode.PhoneNumber,
                          };

            var response = stream.EncryptionSendAndRecieve(requestSignUp, clientSession).Cast<Schema.Auth.TAuthorization>();

            var responseUser = response.User.Cast<TUser>();

            return new ConnectionInfo(responseUser, clientSession, stream);
        }
    }
}