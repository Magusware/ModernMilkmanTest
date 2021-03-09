namespace CustomerService.Database
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Addresses")]
    public class Address
    {
        public Guid AddressId { get; set; }

        [Required]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required]
        public string Town { get; set; }

        public string County { get; set; }

        [Required]
        public string Postcode { get; set; }

        public string Country { get; set; }

        public bool PrimaryAddress { get; set; }
    }
}
