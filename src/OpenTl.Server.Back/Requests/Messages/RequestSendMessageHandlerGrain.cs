namespace OpenTl.Server.Back.Requests.Messages
{
    using System;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Messages;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Contracts.Requests.Messages;
    using OpenTl.Server.Back.DAL.Interfaces;
    using OpenTl.Server.Back.Entities;
    using OpenTl.Server.Back.Sessions.Interfaces;

    public class RequestSendMessageHandlerGrain : BaseObjectHandlerGrain<RequestSendMessage, IUpdates>, IRequestSendMessageHandler
    {
        private readonly IUserService _userService;

        private readonly ISessionStore _sessionStore;

        private readonly IRepository<Message> _messageRepository;

        public RequestSendMessageHandlerGrain(IUserService userService, ISessionStore sessionStore, IRepository<Message> messageRepository)
        {
            _userService = userService;
            _sessionStore = sessionStore;
            _messageRepository = messageRepository;
        }
        
        protected override Task<IUpdates> HandleProtected(ulong keyId, RequestSendMessage obj)
        {
            var session =  _sessionStore.GetSession(keyId);
            var currentUser = _userService.GetById(session.CurrentUserId);

            if (obj.Peer is TInputPeerUser peer)
            {
                _messageRepository.Create(new Message
                                          {
                                              Content = obj.Message,
                                              Date = DateTime.UtcNow,
                                              FromUserId = currentUser.UserId,
                                              ToUserId =peer.UserId 
                                          });
            }
            else
            {
                throw new NotSupportedException();
            }

            var response = new TUpdates
                           {
                               Chats = new TVector<IChat>(),
                               Updates = new TVector<IUpdate>(),
                               Users = new TVector<IUser>(),
                               Date = 0,
                               Seq = 0
                           };

            return Task.FromResult(response.Cast<IUpdates>());
        }
    }
}