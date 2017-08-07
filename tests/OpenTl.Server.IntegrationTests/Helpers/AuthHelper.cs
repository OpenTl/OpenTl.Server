namespace OpenTl.Server.IntegrationTests.Helpers
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using OpenTl.Common.Auth;
    using OpenTl.Common.Auth.Client;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    public static class AuthHelper
    {
        internal const string PublicKey = 
            @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7Kh5FpK5KxFNFUSQ8yWK
GzBW4OJX+ju3S2zX179uJFqisgaC256UgI4UsUfZhR38zi6g4AZqlOUOZcLNwp3r
6zK0ujxmZNu1M3LQLcS1D4aoWHZRHnEz0wuZqhOnIOXA31ATB8kCqiIu0FbKrZ/5
HnPfdDFXt6bfv3+vgyyJwI/G8umiLHo0DSBuXnxo4v6/9hEQcj8acZ26sNj3u9N8
M1WncvMct9os0FJWKaeJ0BUatxHrZC/xsaW5nS9f6Pjw9TfMwuU9qnEZye4Gmgu8
6lv1fbUcg0zl0S4FQpDnf0aKOaU2+DiNxM6DqCBJs7Fz9OZ+LB7bw+5006/defll
cQIDAQAB
-----END PUBLIC KEY-----";

        internal static void InitConnection(out NetworkStream networkStream, out int mesSeqNumber, out AuthKey authKey, out int serverTime)
        {
            var connectTask = AuthHelper.GetServerStream();
            connectTask.Wait();
            
            networkStream = connectTask.Result;

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            
            var resPq = AuthHelper.GetStep1Response(networkStream, requestReqPq, mesSeqNumber);
            mesSeqNumber++;

            var serverDhParams =  AuthHelper.GetStep2Response(resPq, networkStream, mesSeqNumber, out var newNonce);
            mesSeqNumber++;

            var response = AuthHelper.GetStep3Response(networkStream, serverDhParams, mesSeqNumber, newNonce, out var clientAgree, out var time);
            mesSeqNumber++;
            
            authKey = new AuthKey(clientAgree);
            serverTime = time;
        }
        
        internal static TDhGenOk GetStep3Response(Stream networkStream, TServerDHParamsOk serverDhParams, int mesSeqNumber, byte[] newNonce, out byte[] clientAgree, out int serverTime)
        {
            var reqDhParams = Step3ClientHelper.GetRequest(serverDhParams, newNonce, out var agree, out var time);
            clientAgree = agree;
            serverTime = time;

            var reqDhParamsData = Serializer.SerializeObject(reqDhParams);
            var reqMessage = NetworkHelper.EncodeMessage(reqDhParamsData, mesSeqNumber);

            networkStream.Write(reqMessage, 0, reqMessage.Length);
            
            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                var resMessage = NetworkHelper.DecodeMessage(networkStream, out var seqNum, out var checksum);
                return (TDhGenOk) Serializer.DeserializeObject(resMessage);
            }
        }

        internal static TServerDHParamsOk  GetStep2Response(TResPQ resPq, Stream networkStream, int mesSeqNumber,  out byte[] clientNonce)
        {
            var reqDhParams = Step2ClientHelper.GetRequest(resPq, PublicKey, out var newNonce);
            clientNonce = newNonce;

            var reqDhParamsData = Serializer.SerializeObject(reqDhParams);
            var reqMessage = NetworkHelper.EncodeMessage(reqDhParamsData, mesSeqNumber);

            networkStream.Write(reqMessage, 0, reqMessage.Length);
            
            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                var resMessage = NetworkHelper.DecodeMessage(networkStream, out var seqNum, out var checksum);
                return (TServerDHParamsOk) Serializer.DeserializeObject(resMessage);
            }
        }

        internal static TResPQ GetStep1Response(Stream networkStream, RequestReqPq resPq, int mesSeqNumber)
        {
            var resPqData = Serializer.SerializeObject(resPq);

            var reqMessage = NetworkHelper.EncodeMessage(resPqData, mesSeqNumber);
            
            networkStream.Write(reqMessage, 0, reqMessage.Length);

            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                var resMessage = NetworkHelper.DecodeMessage(networkStream, out var seqNum, out var checksum);
                return (TResPQ)Serializer.DeserializeObject(resMessage);
            }
        }

        internal static async Task<NetworkStream> GetServerStream()
        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            var networkStream = client.GetStream();
            return networkStream;
        }
    }
}