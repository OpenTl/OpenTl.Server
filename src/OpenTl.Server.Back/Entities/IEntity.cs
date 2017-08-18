namespace OpenTl.Server.Back.Entities
{
    using System;

    public interface IEntity
    {
        Guid Id { get; set; }
    }
}