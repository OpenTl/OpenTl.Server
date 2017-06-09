using System.IO;
using System.Threading.Tasks;
using BarsGroup.CodeGuard;
using OpenTl.Server.Back.Contracts;
using Orleans.Runtime;

namespace OpenTl.Server.Back
{
    public class HelloPackageHandlerGrain : Orleans.Grain, IHelloPackageHandler
    {
        private int _i;

        public Task<byte[]> Handle(byte[] package)
        {
            GetLogger().Info($"{++_i}");

            string greeting;

            using (var stream = new MemoryStream(package))
            using (var reader = new BinaryReader(stream))
            {
                Guard.That(reader.ReadInt32()).IsEqual(1);

                greeting = reader.ReadString();
            }

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(2);
                writer.Write($"{greeting}, Hello!");

                return Task.FromResult(stream.ToArray());
            }
        }
    }
}