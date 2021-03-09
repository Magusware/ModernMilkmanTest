namespace CustomerService.Database
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;

    public class CustomerDataStore : ICustomerDataStore
    {
        public Task AddAddressAsync(Interfaces.Address address, Guid customerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAddressAsync(Guid addressId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Interfaces.Customer> GetCustomerAsync(object customerId, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Interfaces.Customer>> GetCustomersAsync(bool includeInactive, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAddressAsync(Interfaces.Address address, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCustomerAsync(Interfaces.Customer customer, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
