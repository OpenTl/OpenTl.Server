using System;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Server.Back.Contracts;
using Orleans;

namespace OpenTl.Server.Back
{
    public abstract class BaseObjectHandlerGrain<TInput, TOut>: Grain, IObjectHandler where TInput: IObject where TOut: IObject
    {
        public async Task<IObject> Handle(Guid clientId, IObject obj)
        {
            return await HandleProtected(clientId, (TInput) obj);
        }

        protected abstract Task<TOut> HandleProtected(Guid clientId, TInput obj);
    }
}