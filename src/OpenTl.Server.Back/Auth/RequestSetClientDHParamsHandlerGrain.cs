namespace OpenTl.Server.Back.Auth
 {
     using System;
     using System.Threading.Tasks;

     using OpenTl.Common.Auth;
     using OpenTl.Common.Auth.Server;
     using OpenTl.Schema;
     using OpenTl.Server.Back.Cache;
     using OpenTl.Server.Back.Contracts.Auth;
     using OpenTl.Server.Back.Sessions;

     public class RequestSetClientDhParamsHandlerGrain: BaseObjectHandlerGrain<RequestSetClientDHParams, ISetClientDHParamsAnswer>, IRequestSetClientDhParamsHandler
     {
         protected override Task<ISetClientDHParamsAnswer> HandleProtected(Guid clientId, RequestSetClientDHParams obj)
         {
             var cache = AuthCache.GetCache(clientId);
 
             var response = Step3ServerHelper.GetResponse(obj, cache.NewNonse, cache.KeyPair, out var serverAgree, out var serverSalt);

             //TODO: set sessionId
             var session = new ServerSession
                           {
                               AuthKey = new AuthKey(serverAgree.ToByteArray()),
                               ServerSalt = serverSalt,
                               SessionId = 0
                           };
             SessionStore.SetSession(session);
             
             return Task.FromResult(response);
         }
     }
 }