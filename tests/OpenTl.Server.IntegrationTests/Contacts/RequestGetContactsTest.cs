namespace OpenTl.Server.IntegrationTests.Contacts
{
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Server.IntegrationTests.Helpers;

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
    }
}