namespace OpenTl.Server.Back.Requests.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Contracts.Requests.Contacts;
    using OpenTl.Server.Back.DAL.Interfaces;

    public class RequestImportContactsAutomapperProfile: Profile
    {
        public RequestImportContactsAutomapperProfile(IUserService userService)
        {
            CreateMap<IInputContact, Contact>()
                .ForMember(contact => contact.FirstName, expression => expression.MapFrom(contact => contact.FirstName))
                .ForMember(contact => contact.LastName, expression => expression.MapFrom(contact => contact.LastName))
                .ForMember(contact => contact.PhoneNumber, expression => expression.MapFrom(contact => contact.Phone))
                .ForMember(contact => contact.UserId, expression => expression.MapFrom(contact =>  userService.GetByPhone(contact.Phone).UserId))
                .ForAllOtherMembers(expression => expression.Ignore());
        }
    }

    public class RequestImportContactsHandlerGrain: BaseObjectHandlerGrain<RequestImportContacts, IImportedContacts>, IRequestImportContactsHandler
    {
        private readonly IUserService _userService;

        private readonly IRepository<ServerSession> _sessionStore;

        public RequestImportContactsHandlerGrain(IUserService userService, IRepository<ServerSession> sessionStore)
        {
            _userService = userService;
            _sessionStore = sessionStore;
        }

        protected override Task<IImportedContacts> HandleProtected(Guid keyId, RequestImportContacts obj)
        {
            var session =  _sessionStore.Get(keyId);

            var currentUser = _userService.GetById(session.UserId);

            if (obj.Replace)
            {
                throw new NotSupportedException();
            }

            var newContacts = new List<Contact>();
            var importedContacts = new List<TImportedContact>();

            foreach (var contactsItem in obj.Contacts.Items)
            {
                var newContact = Mapper.Map<Contact>(contactsItem);

                newContacts.Add(newContact);
                importedContacts.Add(new TImportedContact
                                     {
                                         UserId = newContact.UserId,
                                         ClientId = contactsItem.ClientId
                                     });
            }

            currentUser.Contacts = currentUser.Contacts.ToArray().Union(newContacts).ToArray();
            currentUser.Users = currentUser.Users.ToArray().Union(newContacts.Select(c => c.UserId)).ToArray();

            return Task.FromResult<IImportedContacts>(new TImportedContacts
                                                      {
                                                          Imported = new TVector<IImportedContact>(importedContacts.ToArray()),
                                                          PopularInvites = new TVector<IPopularContact>(),
                                                          RetryContacts = new TVector<long>(),
                                                          Users = new TVector<IUser>(Mapper.Map<IUser[]>(currentUser.Users))
            });
        }
    }
}