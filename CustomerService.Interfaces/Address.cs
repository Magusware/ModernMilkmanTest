namespace CustomerServiceNS.Interfaces
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

        public bool IsValid
        {
            get
            {
                // Check if required fields are here
                if (string.IsNullOrEmpty(this.AddressLine1) ||
                    string.IsNullOrEmpty(this.Town) ||
                    string.IsNullOrEmpty(this.Postcode))
                {
                    return false;
                }

                // Check lengths on fields
                if (this.AddressLine1.Length > 80 ||
                    (!string.IsNullOrEmpty(this.AddressLine2) && this.AddressLine2.Length > 80) ||
                    this.Town.Length > 50 ||
                    (!string.IsNullOrEmpty(this.County) && this.AddressLine2.Length > 50) ||
                    this.Postcode.Length > 10)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
