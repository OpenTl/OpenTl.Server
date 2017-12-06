namespace OpenTl.Server.Back.Updates
{
    using System.Linq;
    using System.Threading.Tasks;

    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.Back.Contracts;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.DAL.Interfaces;

    using Orleans;

    public class UpdatesGrain : Grain, IUpdatesGrain
    {
        private readonly IRepository<ServerSession> _sessionRepository;

        public UpdatesGrain(IRepository<ServerSession> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }
        
        public Task NotyfyAboutRecieveMessage(User user, Message message)
        {
            var sessionsIDs = _sessionRepository.GetAll().Where(s => s.UserId == user.UserId).Select(s => s.Id);
            
            var streamProvider = GetStreamProvider("UpdatesNotifocation");
            var stream = streamProvider.GetStream<byte[]>(StreamKeys.UpdatesNotificationKey, "UpdatesNotifocation" );


            var update = new TUpdateShortChatMessage
                         {
                             ChatId = user.UserId,
                             Date = message.Date,
                             Message = message.Content,
                             Entities = new TVector<IMessageEntity>(),
                             FwdFrom = new TMessageFwdHeader()
                         };
            var updateData = Serializer.SerializeObject(update);

            stream.OnNextAsync(updateData);
            
            return Task.FromResult(true);
        }
    }
}