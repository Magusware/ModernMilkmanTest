namespace CustomerServiceNS.Database
{
    using System;
    using System.Linq;

    public class CustomerGetOptions
    {
        public Guid? CustomerId { get; set; }

        public string EmailAddress { get; set; }

        public IQueryable<Customer> ApplyOptions(IQueryable<Customer> customers)
        {
            if (this.CustomerId.HasValue)
            {
                customers = customers.Where(x => x.CustomerId == this.CustomerId);
            }

            if (!string.IsNullOrWhiteSpace(this.EmailAddress))
            {
                customers = customers.Where(x => x.EmailAddress == this.EmailAddress);
            }

            return customers;
        }
    }
}
