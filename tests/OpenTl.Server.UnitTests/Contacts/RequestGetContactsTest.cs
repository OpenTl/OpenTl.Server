namespace OpenTl.Server.UnitTests.Contacts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Moq;

    using OpenTl.Common.Interfaces;
    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Maps;
    using OpenTl.Server.Back.Requests.Contacts;
    using OpenTl.Server.Back.Sessions.Interfaces;

    using Ploeh.AutoFixture;

    using Xunit;

    public class RequestGetContactsTest: BaseTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var userLists = new List<User>();

            var currentUser = Fixture.Create<User>();
            userLists.Add(currentUser);
            userLists.AddRange(Fixture.CreateMany<User>(6));

            currentUser.Users = userLists.Skip(1).Take(3).Select(u => u.UserId);
            currentUser.Contacts = userLists.Skip(1).Skip(3).Select(u => u.UserId);

            var authKeyId = Fixture.Create<ulong>();

            var mSession = Fixture.Freeze<Mock<ISession>>();
            mSession.Setup(s => s.CurrentUserId).Returns(currentUser.UserId);
            var session = mSession.Object;

            var mSessionStore = new Mock<ISessionStore>();
            mSessionStore.Setup(service => service.TryGetSession(authKeyId, out session));

            var mUserService = new Mock<IUserService>();
            mUserService.Setup(service => service.GetById(It.IsAny<int>())).Returns<int>(userId => userLists.Find(u => u.UserId == userId));

            var handler = new RequestGetContactsHandlerGrain(mUserService.Object, mSessionStore.Object);
            
            Mapper.AssertConfigurationIsValid();

            currentUser  = new User();
            Mapper.Map<User, TUser>(currentUser);

            //var request = new RequestGetContacts();
            //var requestData = Serializer.SerializeObject(request);

            //var responseData = await handler.Handle(authKeyId, requestData);
            //var response = Serializer.DeserializeObject(responseData).Cast<TContacts>();

        }
    }
}