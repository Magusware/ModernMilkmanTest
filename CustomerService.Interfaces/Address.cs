namespace CustomerService.Interfaces
{
    using System;

    public class Address
    {
        public Guid AddressId { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string Town { get; set; }

        public string County { get; set; }

        public string Postcode { get; set; }

        public string Country { get; set; }
    }
}
