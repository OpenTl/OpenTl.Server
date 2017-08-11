namespace OpenTl.Server.IntegrationTests.Helpers
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using OpenTl.Common.Auth;
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.Interfaces;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.IntegrationTests.Entities;

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

        internal static void InitConnection(out NetworkStream networkStream, ref int seqNumber, out ISession session, out int serverTime)
        {
            session = new TestSession();
            
            var connectTask = NetworkHelper.GetServerStream();
            connectTask.Wait();
            
            networkStream = connectTask.Result;

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            
            var resPq = GetStep1Response(networkStream, requestReqPq, seqNumber);
            seqNumber++;

            var serverDhParams =  GetStep2Response(resPq, networkStream, seqNumber, out var newNonce);
            seqNumber++;

            var response = GetStep3Response(networkStream, serverDhParams, seqNumber, newNonce, out var clientAgree, out var time);
            seqNumber++;
            
            session.AuthKey = new AuthKey(clientAgree);
            session.ServerSalt = SaltHelper.ComputeServerSalt(newNonce, serverDhParams.ServerNonce);
            
            serverTime = time;
        }
        
        internal static TDhGenOk GetStep3Response(NetworkStream networkStream, TServerDHParamsOk serverDhParams, int seqNumber, byte[] newNonce, out byte[] clientAgree, out int serverTime)
        {
            var reqDhParams = Step3ClientHelper.GetRequest(serverDhParams, newNonce, out var agree, out var time);
            clientAgree = agree;
            serverTime = time;

            var package = Serializer.SerializeObject(reqDhParams);
            var response = networkStream.SendAndRecive(package, seqNumber);
            return Serializer.DeserializeObject(response).Cast<TDhGenOk>();
        }

        internal static TServerDHParamsOk  GetStep2Response(TResPQ resPq, NetworkStream networkStream, int seqNumber,  out byte[] clientNonce)
        {
            var reqDhParams = Step2ClientHelper.GetRequest(resPq, PublicKey, out var newNonce);
            clientNonce = newNonce;

            var package = Serializer.SerializeObject(reqDhParams);
            var response = networkStream.SendAndRecive(package, seqNumber);
            return Serializer.DeserializeObject(response).Cast<TServerDHParamsOk>();
        }

        internal static TResPQ GetStep1Response(NetworkStream networkStream, RequestReqPq resPq, int seqNumber)
        {
            var package = Serializer.SerializeObject(resPq);
            var response = networkStream.SendAndRecive(package, seqNumber);
            return Serializer.DeserializeObject(response).Cast<TResPQ>();
        }       
    }
}