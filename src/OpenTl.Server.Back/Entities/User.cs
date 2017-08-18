namespace OpenTl.Server.Back.Entities
{
    using System;

    public class User: IEntity
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }
        
        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}