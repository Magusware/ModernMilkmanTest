namespace CustomerService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Interfaces;

    public class CustomerService : ICustomerService
    {
        private readonly Database.ICustomerDataStore dataStore;

        public CustomerService(Database.ICustomerDataStore dataStore)
        {
            this.dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public async Task<bool> AddCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            try
            {
                if (!await this.CustomerExists(customer.CustomerId, cancellationToken).ConfigureAwait(false))
                {
                    if (!ValidateCustomerAddresses(customer))
                    {
                        // Maybe return something better than
                        // bool in future to indicate the fault.
                        return false;
                    }

                    await this.dataStore.AddCustomerAsync(customer, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }

            return true;
        }

        public async Task<Customer> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                return await dataStore.GetCustomerAsync(customerId, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }
        }

        public async Task<bool> DeleteCustomerAsync(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.CustomerExists(customerId, cancellationToken).ConfigureAwait(false))
                {
                    await this.dataStore.DeleteCustomerAsync(customerId, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }

            return true;
        }

        public async Task<IEnumerable<Customer>> GetCustomersAsync(bool includeInactive, CancellationToken cancellationToken)
        {
            try
            {
                return await dataStore.GetCustomersAsync(includeInactive, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer, CancellationToken cancellationToken)
        {
            try
            {
                if (await this.CustomerExists(customer.CustomerId, cancellationToken).ConfigureAwait(false))
                {
                    if (!ValidateCustomerAddresses(customer))
                    {
                        return false;
                    }

                    await this.dataStore.UpdateCustomerAsync(customer, cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // To do: Log exception
                throw;
            }

            return true;
        }

        private static bool ValidateCustomerAddresses(Customer customer)
        {
            if (customer.PrimaryAddress == null)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> CustomerExists(Guid customerId, CancellationToken cancellationToken)
        {
            try
            {
                // This logic can be swapped out with something more efficient
                // like a cache of customers or a better Db lookup.
                var customer = await this.dataStore.GetCustomerAsync(customerId, cancellationToken).ConfigureAwait(false);
                return customer != null;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to check if customer exists", ex);
            }
        }
    }
}
