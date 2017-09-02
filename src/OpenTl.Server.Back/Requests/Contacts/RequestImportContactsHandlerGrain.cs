﻿namespace OpenTl.Server.Back.Requests.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using OpenTl.Schema;
    using OpenTl.Schema.Contacts;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Contracts.Requests.Contacts;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Sessions.Interfaces;

    public class RequestImportContactsAutomapperProfile: Profile
    {
        public RequestImportContactsAutomapperProfile()
        {
            CreateMap<IInputContact, Contact>()
                .ForMember(contact => contact.FirstName, expression => expression.MapFrom(contact => contact.FirstName))
                .ForMember(contact => contact.LastName, expression => expression.MapFrom(contact => contact.LastName))
                .ForMember(contact => contact.PhoneNumber, expression => expression.MapFrom(contact => contact.Phone))
                .ForMember(contact => contact.UserId, expression => expression.Ignore())
                .ForAllOtherMembers(expression => expression.Ignore());
        }
    }
    public class RequestImportContactsHandlerGrain: BaseObjectHandlerGrain<RequestImportContacts, IImportedContacts>, IRequestImportContactsHandler
    {
        private readonly IUserService _userService;

        private readonly ISessionStore _sessionStore;

        public RequestImportContactsHandlerGrain(IUserService userService, ISessionStore sessionStore)
        {
            _userService = userService;
            _sessionStore = sessionStore;
        }

        protected override Task<IImportedContacts> HandleProtected(ulong keyId, RequestImportContacts obj)
        {
            var session =  _sessionStore.GetSession(keyId);

            var currentUser = _userService.GetById(session.CurrentUserId);

            if (obj.Replace)
            {
                throw new NotSupportedException();
            }

            var newContacts = new List<Contact>();
            var importedContacts = new List<TImportedContact>();

            foreach (var contactsItem in obj.Contacts.Items)
            {
                var newContact = Mapper.Map<Contact>(contactsItem);

                var user = _userService.GetByPhone(newContact.PhoneNumber);
                newContact.UserId = user.UserId;

                newContacts.Add(newContact);
                importedContacts.Add(new TImportedContact
                                     {
                                         UserId = user.UserId,
                                         ClientId = contactsItem.ClientId
                                     });
            }

            var contacts = Mapper.Map<Contact[]>(obj.Contacts.Items);

            currentUser.Contacts = currentUser.Contacts.ToArray().Union(contacts);

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