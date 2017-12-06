namespace OpenTl.Server.Back.Requests.Messages
{
    using System;
    using System.Runtime.InteropServices.ComTypes;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Messages;
    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Contracts.Requests.Messages;
    using OpenTl.Server.Back.DAL.Interfaces;

    public class RequestSendMessageHandlerGrain : BaseObjectHandlerGrain<RequestSendMessage, IUpdates>, IRequestSendMessageHandler
    {
        private readonly IUserService _userService;

        private readonly IRepository<ServerSession> _sessionStore;

        private readonly IRepository<Message> _messageRepository;

        public RequestSendMessageHandlerGrain(IUserService userService, IRepository<ServerSession> sessionStore, IRepository<Message> messageRepository)
        {
            _userService = userService;
            _sessionStore = sessionStore;
            _messageRepository = messageRepository;
        }
        
        protected override Task<IUpdates> HandleProtected(Guid keyId, RequestSendMessage obj)
        {
            var session =  _sessionStore.Get(keyId);
            var currentUser = _userService.GetById(session.UserId);

            if (obj.Peer is TInputPeerUser peer)
            {
                _messageRepository.Create(new Message
                                          {
                                              Content = obj.Message,
                                              Date = (int)((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds(),
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