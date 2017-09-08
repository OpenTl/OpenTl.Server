namespace OpenTl.Server.UnitTests.Builders
{
    using System.Linq;

    using Moq;

    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Entities;

    public static class UserServiceBuilder
    {
        public static void BuildUserService(this BaseTest baseTest, User[] users)
        {
            var mUserService = new Mock<IUserService>();
            mUserService.Setup(service => service.GetById(It.IsAny<int>())).Returns<int>(userId => users.Single(u => u.UserId == userId));
            mUserService.Setup(service => service.GetByPhone(It.IsAny<string>())).Returns<string>(phone => users.Single(u => u.PhoneNumber == phone));
            
            baseTest.RegisterMockAndInstance(mUserService);
        }
    }
}