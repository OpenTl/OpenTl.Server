namespace OpenTl.Server.UnitTests.Contacts
{
    using System.Threading.Tasks;

    using AutoMapper;

    using OpenTl.Schema;
    using OpenTl.Server.Back.Entities;

    using Xunit;

    public class SimpleTests
    {
        [Fact]
        public async Task SimpleTest()
        {
            Mapper.Initialize(
                cfg =>
                {
                });

            var user = new User
                       {
                           FirstName = "123123",
                           LastName = "asdasd",
                           PhoneNumber = "123123",
                           UserId = 123
                       };
            var tuser = Mapper.Map<TUser>(user);

        }
    }
}