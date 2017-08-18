namespace OpenTl.Server.IntegrationTests
{
    using System.Net.Sockets;

    using OpenTl.Common.Interfaces;
    using OpenTl.Common.MtProto;
    using OpenTl.Schema;
    using OpenTl.Schema.Help;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.IntegrationTests.Helpers;

    public abstract class BaseTestWithEncryption
    {
        private int _seqNumber = 0;

        private NetworkStream _networkStream;

        private ISession _session;

        private int _serverTime;

        protected BaseTestWithEncryption()
        {
            InitConnection();
        }
        
        public void InitConnection()
        {
            AuthHelper.Handshake(out var stream, ref _seqNumber, out var clientSession, out int time);
            _networkStream = stream;
            _session = clientSession;
            _serverTime = time;
            
            var request = new RequestInvokeWithLayer
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
            

            SendEndRecieve(request).Cast<IConfig>();
        }

        protected IObject SendEndRecieve(IObject request)
        {
            var requestData = Serializer.SerializeObject(request);
            var encryptedRequestData = MtProtoHelper.FromClientEncrypt(requestData, _session, _seqNumber);
            _seqNumber++;

            var encryptedResponseData = _networkStream.SendAndRecive(encryptedRequestData, _seqNumber);
            var responseData = MtProtoHelper.FromServerDecrypt(encryptedResponseData, _session, out var authKeyId, out var serverSalt, out var sessionId, out var messageId, out var sNumber);
            
            return Serializer.DeserializeObject(responseData);
        }
    }
}