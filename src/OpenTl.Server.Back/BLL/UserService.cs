namespace OpenTl.Server.Back.BLL
{
    using System.Linq;

    using OpenTl.Server.Back.BLL.Interfaces;
    using OpenTl.Server.Back.DAL.Interfaces;
    using OpenTl.Server.Back.Entities;

    public class UserService : IUserService
    {
        private readonly IRepository<User> _repository;

        public UserService(IRepository<User> repository)
        {
            _repository = repository;
        }

        public User GetById(int userId)
        {
            return _repository.GetAll().Single(u => u.UserId == userId);
        }

        public User GetByPhone(string phoneNumber)
        {
            return _repository.GetAll().Single(u => u.PhoneNumber == phoneNumber);
        }
    }
}