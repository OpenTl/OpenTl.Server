namespace OpenTl.Server.Back.Auth
 {
     using System;
     using System.Threading.Tasks;

     using OpenTl.Common.Auth;
     using OpenTl.Common.Auth.Server;
     using OpenTl.Schema;
     using OpenTl.Server.Back.Cache;
     using OpenTl.Server.Back.Contracts.Auth;
     using OpenTl.Server.Back.Contracts.Entities;
     using OpenTl.Server.Back.DAL.Interfaces;

     public class RequestSetClientDhParamsHandlerGrain: BaseObjectHandlerGrain<RequestSetClientDHParams, ISetClientDHParamsAnswer>, IRequestSetClientDhParamsHandler
     {
         private readonly IRepository<ServerSession> _sessionStore;

         public RequestSetClientDhParamsHandlerGrain(IRepository<ServerSession> sessionStore)
         {
             _sessionStore = sessionStore;
         }

         protected override Task<ISetClientDHParamsAnswer> HandleProtected(Guid keyId, RequestSetClientDHParams obj)
         {
             var cache = AuthCache.GetCache(keyId);
 
             var response = Step3ServerHelper.GetResponse(obj, cache.NewNonse, cache.KeyPair, out var serverAgree, out var serverSalt);

             //TODO: set sessionId
             
             var authKey = new AuthKey(serverAgree.ToByteArrayUnsigned());
             
             var session = new ServerSession
                           {
                               AuthKey = authKey,
                               ServerSalt = serverSalt,
                               SessionId = 0,
                               Id = authKey.ToGuid() 
                           };
             _sessionStore.Create(session);
             
             return Task.FromResult(response);
         }
     }
 }