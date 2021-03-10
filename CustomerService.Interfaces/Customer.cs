namespace CustomerServiceNS.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public bool IsActive { get; set; }

        public bool IsValid
        {
            get
            {
                // Validatiion can be improved by inclusion of a library
                // which will handle required properties automatically
                if (string.IsNullOrEmpty(this.Title) ||
                    string.IsNullOrEmpty(this.Forename) ||
                    string.IsNullOrEmpty(this.Surname) ||
                    string.IsNullOrEmpty(this.EmailAddress) ||
                    string.IsNullOrEmpty(this.MobileNumber))
                {
                    return false;
                }

                if (this.Title.Length > 20 ||
                    this.Forename.Length > 50 || 
                    this.Surname.Length > 50 ||
                    this.EmailAddress.Length > 75 ||
                    this.MobileNumber.Length > 15)
                {
                    return false;
                }

                if ((!this.PrimaryAddress?.IsValid) ?? true ||
                    (this.SecondaryAddresses?.Any(x => !x.IsValid) ?? false))
                {
                    return false;
                }

                return true;
            }
        }

        public bool HasAddress(Address address) =>
            this.PrimaryAddress.IsAddress(address) ||
            this.SecondaryAddresses.Any(x => x.IsAddress(address));
    }
}
