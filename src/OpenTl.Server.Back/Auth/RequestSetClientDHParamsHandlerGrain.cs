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
     using OpenTl.Server.Back.Sessions.Interfaces;

     public class RequestSetClientDhParamsHandlerGrain: BaseObjectHandlerGrain<RequestSetClientDHParams, ISetClientDHParamsAnswer>, IRequestSetClientDhParamsHandler
     {
         private readonly ISessionStore _sessionStore;

         public RequestSetClientDhParamsHandlerGrain(ISessionStore sessionStore)
         {
             _sessionStore = sessionStore;
         }

         protected override Task<ISetClientDHParamsAnswer> HandleProtected(ulong keyId, RequestSetClientDHParams obj)
         {
             var cache = AuthCache.GetCache(keyId);
 
             var response = Step3ServerHelper.GetResponse(obj, cache.NewNonse, cache.KeyPair, out var serverAgree, out var serverSalt);

             //TODO: set sessionId
             var session = new ServerSession
                           {
                               AuthKey = new AuthKey(serverAgree.ToByteArray()),
                               ServerSalt = serverSalt,
                               SessionId = 0
                           };
             _sessionStore.SetSession(session);
             
             return Task.FromResult(response);
         }
     }
 }