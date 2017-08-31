namespace OpenTl.Server.UnitTests.Contacts
{
    using System.Threading.Tasks;

    using AutoMapper;

    using OpenTl.Schema;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Maps;

    using Xunit;

    public class SimpleTests
    {
        [Fact]
        public async Task SimpleTest()
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<User, TUser>()
                       .ForMember(u => u.FirstName, expression => expression.MapFrom(u => u.FirstName))
                       .ForMember(u => u.LastName, expression => expression.MapFrom(u => u.LastName))
                       .ForMember(u => u.Phone, expression => expression.MapFrom(u => u.PhoneNumber))
                       .ForMember(u => u.Id, expression => expression.MapFrom(u => u.UserId))
                       .IgnoreAllUnmapped();
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