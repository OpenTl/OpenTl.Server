namespace OpenTl.Server.Back.BLL.Interfaces
{
    using OpenTl.Server.Back.Entities;

    public interface IUserService
    {
        User GetById(int userId);

        User GetByPhone(string phoneNumber);
    }
}