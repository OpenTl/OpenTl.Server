using System;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Server.Back.Contracts;
using Orleans;

namespace OpenTl.Server.Back
{
    using OpenTl.Schema.Serialization;

    public abstract class BaseObjectHandlerGrain<TInput, TOut>: Grain, IObjectHandler where TInput: IObject where TOut: IObject
    {
        public async Task<byte[]> Handle(ulong keyId, byte[] package)
        {
            var request = Serializer.DeserializeObject(package);

            var response = await HandleProtected(keyId, (TInput) request);

            return Serializer.SerializeObject(response);
        }

        protected abstract Task<TOut> HandleProtected(ulong keyId, TInput obj);
    }
}