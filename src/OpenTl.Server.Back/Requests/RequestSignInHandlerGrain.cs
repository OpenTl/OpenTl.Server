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

    public class RequestSignInHandlerGrain : BaseObjectHandlerGrain<RequestSignIn, IAuthorization>,
                                             IRequestSignInHandler
    {
        private readonly IRepository<User> _userRepository;

        public RequestSignInHandlerGrain(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        
        protected override Task<IAuthorization> HandleProtected(Guid clientId, RequestSignIn obj)
        {
            Guard.That(obj.PhoneCode).IsEqual("7777");
            Guard.That(obj.PhoneCodeHashAsBinary).IsItemsEquals(SHA1Helper.ComputeHashsum(Encoding.UTF8.GetBytes("7777")));

            var user = _userRepository.GetAll().Single(u => u.PhoneNumber == obj.PhoneNumber);
            
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