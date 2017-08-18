
namespace OpenTl.Server.UnitTests
{
    using System;
    using System.Threading.Tasks;

    using Xunit;

    using OpenTl.Schema;
    using OpenTl.Schema.Help;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Requests;

    public class RequestInitConnectionTest
    {
        [Fact]
        public async Task SimpleTest()
        {
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
            var requestData = Serializer.SerializeObject(request);
            
            var grain = new RequestInvokeWithLayerHandlerGrain();
            var responseData = await grain.Handle(Guid.Empty, requestData);
            
            var response = Serializer.DeserializeObject(responseData).Cast<TConfig>();
            
            Assert.Equal(response.CallConnectTimeoutMs, 1111);
        }
    }
}