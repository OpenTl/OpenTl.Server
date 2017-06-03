using System;
using System.Threading.Tasks;
using OpenTl.Server.Back.Contracts;

namespace OpenTl.Server.Back
{
    public class HelloGrain : Orleans.Grain, IHello
    {
        public Task<string> SayHello(string greeting)
        {
            return Task.FromResult($"{greeting}, Hello!");
        }
    }
}