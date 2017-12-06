namespace OpenTl.Server.Back.Contracts.Entities
{
    using System;

    public class Message: IEntity
    {
        public Guid Id { get; set; }

        public string Content { get; set; }
        
        public int Date { get; set; }

        public int FromUserId { get; set; }
        
        public int ToUserId { get; set; }

    }
}