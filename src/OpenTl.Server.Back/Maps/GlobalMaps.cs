namespace OpenTl.Server.Back.Maps
{
    using AutoMapper;

    using OpenTl.Schema;
    using OpenTl.Server.Back.Entities;

    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;

    public class GlobalProfile : Profile
    {
        public GlobalProfile()
        {
            CreateMap<User, TUser>()
                .ForMember(user => user.FirstName, expression => expression.MapFrom(user => user.FirstName))
                .ForMember(user => user.LastName, expression => expression.MapFrom(user => user.LastName))
                .ForMember(user => user.Phone, expression => expression.MapFrom(user => user.PhoneNumber))
                .ForMember(user => user.Id, expression => expression.MapFrom(user => user.UserId))
                .ForAllOtherMembers(expression => expression.Ignore());

            CreateMap<User, IUser>()
                .As<TUser>();

            CreateMap<User, TAuthorization>()
                .ForMember(authorization => authorization.Flags, expression => expression.Ignore())
                .ForMember(authorization => authorization.TmpSessions, expression => expression.UseValue(0))
                .ForMember(authorization => authorization.User, expression => expression.MapFrom(user => user));
        }
    }
}