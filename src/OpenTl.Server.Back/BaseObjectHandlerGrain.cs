using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Server.Back.Contracts;
using Orleans;

namespace OpenTl.Server.Back
{
    public abstract class BaseObjectHandlerGrain<TInput, TOut>: Grain, IObjectHandler where TInput: IObject where TOut: IObject
    {
        public async Task<IObject> Handle(IObject obj)
        {
            return await HandleProtected((TInput) obj);
        }

        protected abstract Task<TOut> HandleProtected(TInput obj);
    }
}