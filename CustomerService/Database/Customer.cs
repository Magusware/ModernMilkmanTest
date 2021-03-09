namespace CustomerServiceNS.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customers")]
    public class Customer
    {
        public Guid CustomerId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Forename { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        public virtual IEnumerable<Address> Addresses { get; set; } = new HashSet<Address>();
    }
}
