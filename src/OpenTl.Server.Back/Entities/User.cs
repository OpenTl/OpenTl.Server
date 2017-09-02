namespace OpenTl.Server.Back.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class User: IEntity
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public IEnumerable<int> Users { get; set; } = Enumerable.Empty<int>();

        public IEnumerable<Contact> Contacts { get; set; } = Enumerable.Empty<Contact>();

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
    }
}