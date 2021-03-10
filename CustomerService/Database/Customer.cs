namespace CustomerServiceNS.Database
{
    using System;
    using System.Collections.Generic;

    public class Customer
    {
        public Guid CustomerId { get; set; }

        public string Title { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public string EmailAddress { get; set; }

        public string MobileNumber { get; set; }

        public virtual IEnumerable<Address> Addresses { get; set; } = new HashSet<Address>();

        public bool IsActive { get; set; }
    }
}
