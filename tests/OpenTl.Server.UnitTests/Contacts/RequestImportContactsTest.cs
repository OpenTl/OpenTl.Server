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
    using OpenTl.Server.Back.Requests.Contacts;
    using OpenTl.Server.Back.Sessions.Interfaces;
    using OpenTl.Server.UnitTests.Extensions;

    using Ploeh.AutoFixture;

    using Xunit;

    public class RequestImportContactsTest: BaseTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var userLists = new List<User>();

            var currentUser = Fixture.Create<User>();
            userLists.Add(currentUser);

            var otherUsers = Fixture.CreateMany<User>(2);
            userLists.AddRange(otherUsers);

            var authKeyId = Fixture.Create<ulong>();

            var mSession = Fixture.Freeze<Mock<ISession>>();
            mSession.Setup(s => s.CurrentUserId).Returns(currentUser.UserId);
            var session = mSession.Object;

            var mSessionStore = new Mock<ISessionStore>();
            mSessionStore.Setup(service => service.GetSession(authKeyId)).Returns<ulong>(arg => session);
            RegisterMockAndInstance(mSessionStore);

            var mUserService = new Mock<IUserService>();
            mUserService.Setup(service => service.GetById(It.IsAny<int>())).Returns<int>(userId => userLists.Find(u => u.UserId == userId));
            mUserService.Setup(service => service.GetByPhone(It.IsAny<string>())).Returns<string>(phoneNumber => userLists.Find(u => u.PhoneNumber == phoneNumber));
            RegisterMockAndInstance(mUserService);

            RegisterSingleton<RequestImportContactsHandlerGrain>();

            this.RegisterAllMaps();
            Mapper.AssertConfigurationIsValid();

            var handler = Resolve<RequestImportContactsHandlerGrain>();

            var newContacts = otherUsers.Select(
                user => new TInputPhoneContact
                        {
                            ClientId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Phone = user.PhoneNumber
                        })
                 .Cast<IInputContact>()
                 .ToArray();

            var request = new RequestImportContacts
                          {
                              Contacts = new TVector<IInputContact>(newContacts)
                          };
            var requestData = Serializer.SerializeObject(request);

            var responseData = await handler.Handle(authKeyId, requestData);
            var response = Serializer.DeserializeObject(responseData).Cast<TImportedContacts>();

            Assert.All(response.Imported.Items, contact => Assert.Equal(contact.UserId, (int)contact.ClientId));
        }
    }
}