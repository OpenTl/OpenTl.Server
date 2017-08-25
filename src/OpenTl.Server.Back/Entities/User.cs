namespace OpenTl.Server.Back.Entities
{
    using System;
    using System.Collections.Generic;

    public class User: IEntity
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public IEnumerable<int> Users { get; set; } = new List<int>();

        public IEnumerable<int> Contacts { get; set; } = new List<int>();

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}