namespace OpenTl.Server.Back.Requests
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using OpenTl.Common.Crypto;
    using OpenTl.Schema.Auth;
    using OpenTl.Server.Back.Contracts.Entities;
    using OpenTl.Server.Back.Contracts.Requests;
    using OpenTl.Server.Back.DAL.Interfaces;

    public class RequestSendCodeHandlerGrain : BaseObjectHandlerGrain<RequestSendCode, ISentCode>,
                                               IRequestSendCodeHandler
    {
        private readonly IRepository<User> _userRepository;

        public RequestSendCodeHandlerGrain(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        
        protected override Task<ISentCode> HandleProtected(Guid keyId, RequestSendCode obj)
        {
            var result = new TSentCode
                         {
                             Type = new TSentCodeTypeApp(),
                             PhoneCodeHashAsBinary = SHA1Helper.ComputeHashsum(Encoding.UTF8.GetBytes("7777")),
                             PhoneRegistered = _userRepository.GetAll().Any(u => u.PhoneNumber == obj.PhoneNumber),
                             Timeout = 60,
                         };

            return Task.FromResult((ISentCode)result);
        }
    }
}