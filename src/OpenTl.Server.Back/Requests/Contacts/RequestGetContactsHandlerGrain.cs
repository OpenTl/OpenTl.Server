namespace OpenTl.Server.Back.Requests.Contacts
{
    using System.Threading.Tasks;

    using AutoMapper;

    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Contracts.Requests.Contacts;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Maps;
    using OpenTl.Server.Back.Sessions.Interfaces;

    public class RequestGetContactsAutomapperProfile : Profile
    {
        public RequestGetContactsAutomapperProfile(IUserService userService)
        {
            this.MapEnumerableToVector<Contact, IContact>();
            this.MapEnumerableToVector<int, IUser>();

            CreateMap<Contact, IContact>()
               .ForMember(contact => contact.UserId, expression => expression.MapFrom(u => u.UserId))
               .ForMember(contact => contact.Mutual, expression => expression.UseValue(false))
               .As<TContact>();

            this.CreateMap<int, IUser>()
                .ConstructUsing(userId => Mapper.Map<TUser>(userService.GetById(userId)));

            this.CreateMap<User, TContacts>()
               .ForMember(contacts => contacts.Contacts, expression => expression.MapFrom(user => user.Contacts))
               .ForMember(contacts => contacts.Users, expression => expression.MapFrom(user => user.Users));
        }
    }

    public class RequestGetContactsHandlerGrain : BaseObjectHandlerGrain<RequestGetContacts, IContacts>, IRequestGetContactsHandler
    {
        private readonly IUserService _userService;

        private readonly ISessionStore _sessionStore;

        public RequestGetContactsHandlerGrain(IUserService userService, ISessionStore sessionStore)
        {
            _userService = userService;
            _sessionStore = sessionStore;
        }

        protected override Task<IContacts> HandleProtected(ulong keyId, RequestGetContacts obj)
        {
            var session =  _sessionStore.GetSession(keyId);

            var currentUser = _userService.GetById(session.CurrentUserId);

            var response = Mapper.Map<TContacts>(currentUser).Cast<IContacts>();

            return Task.FromResult(response);
        }
    }
}