namespace OpenTl.Server.IntegrationTests.Contacts
{
    using System.Linq;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Server.IntegrationTests.Helpers;

    using Org.BouncyCastle.Ocsp;

    using Xunit;

    public class RequestImportAndGetContactsTest
    {
        [Fact]
        public void New_User_Does_Not_Have_Any_Contacts()
        {
           var conn = ConnectionsHelper.CreateConnection();

            var request = new RequestGetContacts();

            var response = conn.NetworkStream.EncryptionSendAndRecieve(request, conn.Session).Cast<TContacts>();

            Assert.Empty(response.Contacts.Items);
            Assert.Empty(response.Users.Items);
        }
        
        [Fact]
        public void User_Import_Contacts()
        {
            var conns = ConnectionsHelper.CreateConnections(2);

            var ourUser = conns[0];
            var friendUser = conns[1];


            var request = new RequestImportContacts
                          {
                              Contacts = new TVector<IInputContact>(
                                  new TInputPhoneContact
                                  {
                                      ClientId = friendUser.User.Id,
                                      FirstName = friendUser.User.FirstName,
                                      LastName = friendUser.User.LastName,
                                      Phone = friendUser.User.Phone
                                  })
                          };

            var importedContacs = ourUser.NetworkStream.EncryptionSendAndRecieve(request, ourUser.Session);

            Assert.Equal(1, importedContacs.Imported.Items.Count);
            Assert.True(importedContacs.Imported.Items.Any(c => c.ClientId == friendUser.User.Id && c.UserId == friendUser.User.Id));

            Assert.Equal(1, importedContacs.Users.Items.Count);
            Assert.Equal(JsonConvert.SerializeObject(friendUser.User), JsonConvert.SerializeObject(importedContacs.Users[0]));
            
            Assert.Equal(0, importedContacs.PopularInvites.Items.Count);
            Assert.Equal(0, importedContacs.RetryContacts.Items.Count);
        }
    }
}