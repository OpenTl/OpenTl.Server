namespace OpenTl.Server.Back.Contracts.Entities
{
    using System;
    using System.Collections.Generic;

    public class Contact: IEntity
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public string PhoneNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Contact contact && PhoneNumber == contact.PhoneNumber;
        }

        public override int GetHashCode()
        {
            return 1603624094 + EqualityComparer<string>.Default.GetHashCode(PhoneNumber);
        }
    }
}