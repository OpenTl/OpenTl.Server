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
    using OpenTl.Server.UnitTests.Builders;
    using OpenTl.Server.UnitTests.Extensions;

    using Ploeh.AutoFixture;

    using Xunit;

    public class RequestImportContactsTest: BaseTest
    {
        [Fact]
        public async Task SimpleTest()
        {
            var otherUsers = Fixture.CreateMany<User>(2).ToArray();
            var userLists = new List<User>(otherUsers);

            var currentUser = this.BuildUser(new User[0]);
            userLists.Add(currentUser);
            
            this.BuildSessionStore(currentUser);
         
            this.BuildUserService(userLists.ToArray());

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


            var session = Resolve<ISession>();
            var responseData = await handler.Handle(session.AuthKey.Id, requestData);
            
            var response = Serializer.DeserializeObject(responseData).Cast<TImportedContacts>();

            Assert.All(response.Imported.Items, contact => Assert.Equal(contact.UserId, (int)contact.ClientId));
        }
    }
}