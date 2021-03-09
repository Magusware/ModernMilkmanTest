namespace CustomerServiceNS.Interfaces
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

        public Address PrimaryAddress { get; set; }

        public IEnumerable<Address> SecondaryAddresses { get; set; }
    }
}
