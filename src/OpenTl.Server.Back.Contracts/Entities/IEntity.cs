namespace OpenTl.Server.Back.Contracts.Entities
{
    using System;

    public interface IEntity
    {
        Guid Id { get; set; }
    }
}