namespace CustomerServiceNS.Database
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class CustomerDataStore : ICustomerDataStore
    {
        private readonly ICustomerServiceDatabaseMapper mapper;
        private readonly CustomerServiceDbContext dbContext;

        public CustomerDataStore(ICustomerServiceDatabaseMapper mapper, CustomerServiceDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken)
        {
            var dbCustomer = this.mapper.Map<Customer>(customer);

            await this.dbContext.Customers.AddAsync(dbCustomer, cancellationToken).ConfigureAwait(false);
            await this.dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            var existing = this.dbContext.Customers.Find(customerId);
            this.dbContext.Remove(existing);
            await this.dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Interfaces.Customer> GetCustomerAsync(CustomerGetOptions options, CancellationToken cancellationToken)
        {
            Interfaces.Customer customer = null;
            IQueryable<Customer> dbCustomers = this.dbContext
                .Customers
                .Include(x => x.Addresses);

            var dbCustomer = await options.ApplyOptions(dbCustomers)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (dbCustomer != null)
            {
                customer = this.mapper.Map<Interfaces.Customer>(dbCustomer);
            }

            return customer;
        }

        public async Task<bool> CheckCustomerExistsAsync(CustomerGetOptions options, CancellationToken cancellationToken)
        {
            IQueryable<Customer> customers = this.dbContext.Customers;

            customers = options.ApplyOptions(customers);

            return await customers.AnyAsync(cancellationToken).ConfigureAwait(false);
        }

        public Task<IEnumerable<Interfaces.Customer>> GetCustomersAsync(
            bool includeInactive,
            CancellationToken cancellationToken)
        {
            IEnumerable<Customer> dbCustomers = this.dbContext
                .Customers
                .Include(x => x.Addresses);

            if (!includeInactive)
            {
                dbCustomers = dbCustomers.Where(x => x.IsActive);
            }

            var result = this.mapper.Map<IEnumerable<Interfaces.Customer>>(dbCustomers.ToArray());
            return Task.FromResult(result);
        }

        public async Task UpdateCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken)
        {
            var updateCustomer = this.mapper.Map<Customer>(customer);

            var entity = this.dbContext.Customers.Include(x => x.Addresses).FirstOrDefault(x => x.CustomerId == customer.CustomerId);
            this.dbContext.Entry(entity).CurrentValues.SetValues(updateCustomer);

            foreach (var address in updateCustomer.Addresses)
            {
                var addressEntity = entity.Addresses.FirstOrDefault(x => x.AddressId == address.AddressId);
                if (addressEntity == null)
                {
                    this.dbContext.Add(address);
                }
                else
                {
                    this.dbContext.Entry(addressEntity).CurrentValues.SetValues(address);
                }
            }

            foreach (var address in entity.Addresses.Where(x => !updateCustomer.Addresses.Any(y => x.AddressId == y.AddressId)))
            {
                this.dbContext.Remove(address);
            }

            await this.dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
