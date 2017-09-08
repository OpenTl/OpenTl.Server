namespace OpenTl.Server.UnitTests.Builders
{
    using System.Linq;

    using OpenTl.Server.Back.Entities;

    using Ploeh.AutoFixture;

    public static class UserBuilder
    {
        public static User BuildUser(this BaseTest baseTest, params User[] contacts)
        {
            var user = baseTest.Fixture.Create<User>();
            user.Users = contacts.Select(с => с.UserId).ToArray();
            user.Contacts = contacts.Select(
                                                u => new Contact
                                                     {
                                                         UserId = u.UserId,
                                                         PhoneNumber = u.PhoneNumber,
                                                         FirstName = u.FirstName,
                                                         LastName = u.LastName
                                                     })
                                    .ToArray();
            return user;
        }
    }
}