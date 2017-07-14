using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BarsGroup.CodeGuard;
using OpenTl.Common.Auth;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Server.Back.Cache;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Server.Back.Helpers;
using OpenTl.Utils.Crypto;
using OpenTl.Utils.GuardExtentions;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqDhParamsHandlerGrain : BaseObjectHandlerGrain<RequestReqDHParams, IServerDHParams>,
        IRequestReqDhParamsHandler
    {
        protected override Task<IServerDHParams> HandleProtected(Guid clientId, RequestReqDHParams obj)
        {
            var cache = AuthCache.GetCache(clientId);

            cache.Nonce = obj.Nonce;
                
            ReqDhParamsHelper.Server(obj, RsaHelper.PrivateKey, out var parameters);

            return null;
        }
    }
}