namespace OpenTl.Server.Back.Contracts
{
    using System.Threading.Tasks;

    using OpenTl.Server.Back.Contracts.Entities;

    using Orleans;

    public interface IUpdatesGrain: IGrainWithIntegerKey
    {
        Task NotyfyAboutRecieveMessage(User user, Message message);
    }
}