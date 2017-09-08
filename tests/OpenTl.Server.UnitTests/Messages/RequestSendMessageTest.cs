namespace OpenTl.Server.UnitTests.Messages
{
    using System.Collections.Generic;

    using OpenTl.Common.Interfaces;
    using OpenTl.Schema;
    using OpenTl.Schema.Messages;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Requests.Messages;
    using OpenTl.Server.UnitTests.Builders;

    using Ploeh.AutoFixture;

    using Xunit;

    public class RequestSendMessageTest: BaseTest
    {
        [Fact]
        public async void Send_Message_Test()
        {
            var userLists = new List<User>();

            var otherUser = this.BuildUser();
            userLists.Add(otherUser);
            
            var currentUser = this.BuildUser(otherUser);
            userLists.Add(currentUser);
            
            this.BuildSessionStore(currentUser);
         
            this.BuildUserService(userLists.ToArray());
            
            this.BuildRepository<Message>();
            
            RegisterSingleton<RequestSendMessageHandlerGrain>();

            var session = Resolve<ISession>();
            
            var grain = Resolve<RequestSendMessageHandlerGrain>();

            var request = new RequestSendMessage
                          {
                              Message = Fixture.Create<string>(),
                              Peer = new TInputPeerUser
                                     {
                                         UserId = otherUser.UserId
                                     },
                              Entities = new TVector<IMessageEntity>(),
                              ReplyMarkup = new TReplyInlineMarkup
                                            {
                                                Rows = new TVector<IKeyboardButtonRow>()
                                            }
                          };

            var requestData = Serializer.SerializeObject(request);
            var responseData = await grain.Handle(session.AuthKey.Id, requestData);

            var response = Serializer.DeserializeObject(responseData);

            Assert.IsType<TUpdates>(response);
        } 
    }
}