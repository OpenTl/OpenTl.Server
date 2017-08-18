namespace OpenTl.Server.Back.Requests
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using BarsGroup.CodeGuard;

    using MoreLinq;

    using OpenTl.Common.Crypto;
    using OpenTl.Common.GuardExtensions;
    using OpenTl.Schema;
    using OpenTl.Schema.Auth;
    using OpenTl.Server.Back.Contracts.Requests;
    using OpenTl.Server.Back.DAL.Interfaces;
    using OpenTl.Server.Back.Entities;

    using IAuthorization = OpenTl.Schema.Auth.IAuthorization;
    using TAuthorization = OpenTl.Schema.Auth.TAuthorization;

    public class RequestSignUpHandlerGrain : BaseObjectHandlerGrain<RequestSignUp, IAuthorization>,
                                             IRequestSignUpHandler
    {
        private readonly IRepository<User> _userRepository;

        public RequestSignUpHandlerGrain(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        
        protected override Task<IAuthorization> HandleProtected(Guid clientId, RequestSignUp obj)
        {
            Guard.That(obj.PhoneCode).IsEqual("7777");
            Guard.That(obj.PhoneCodeHashAsBinary).IsItemsEquals(SHA1Helper.ComputeHashsum(Encoding.UTF8.GetBytes("7777")));

            var maxUserId =  _userRepository.GetAll().Count() != 0
                                ? _userRepository.GetAll().Select(u => u.UserId).Max() + 1
                                : 1;
                
            var user = new User
                       {
                           Id = Guid.NewGuid(),
                           UserId =  maxUserId,
                           FirstName = obj.FirstName,
                           LastName = obj.LastName,
                           PhoneNumber = obj.PhoneNumber,
                       };
            
            _userRepository.Create(user);
                
            var result = new TAuthorization
                         {
                             User = new TUser
                                    {
                                        FirstName = user.FirstName,
                                        LastName = user.LastName,
                                        Id = user.UserId,
                                        Phone = user.PhoneNumber
                                    }
                         };

            return Task.FromResult((IAuthorization)result);
        }
    }
}