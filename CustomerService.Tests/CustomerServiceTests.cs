namespace CustomerServiceNS.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AutoFixture.Xunit2;
    using Database;
    using FluentAssertions;
    using Moq;
    using Xunit;

    public class CustomerServiceTests
    {
        public class AddCustomerTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldAddCustomer(
                Interfaces.Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                dataStore
                    .Setup(x => x.GetCustomerAsync(customer.CustomerId, tokenSource.Token))
                    .ReturnsAsync((Interfaces.Customer)null);

                var result = await sut.AddCustomerAsync(customer, tokenSource.Token).ConfigureAwait(false);

                dataStore.Verify(x => x.AddCustomerAsync(customer, tokenSource.Token), Times.Once);
                result.Should().BeTrue();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldAddCustomerWithNoSecondaryAddress(
                Interfaces.Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                dataStore
                    .Setup(x => x.GetCustomerAsync(customer.CustomerId, tokenSource.Token))
                    .ReturnsAsync((Interfaces.Customer)null);

                customer.SecondaryAddresses = null;

                var result = await sut.AddCustomerAsync(customer, tokenSource.Token).ConfigureAwait(false);

                dataStore.Verify(x => x.AddCustomerAsync(customer, tokenSource.Token), Times.Once);
                result.Should().BeTrue();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldNotAddCustomerWithNoPrimaryAddress(
                Interfaces.Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                customer.PrimaryAddress = null;

                dataStore
                    .Setup(x => x.GetCustomerAsync(customer.CustomerId, tokenSource.Token))
                    .ReturnsAsync((Interfaces.Customer)null);
 
                var result = await sut.AddCustomerAsync(customer, tokenSource.Token).ConfigureAwait(false);

                dataStore.Verify(x => x.AddCustomerAsync(customer, tokenSource.Token), Times.Never);
                result.Should().BeFalse();
            }
        }

        public class GetCustomerTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldGetCustomer(
                Interfaces.Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                dataStore
                    .Setup(x => x.GetCustomerAsync(customer.CustomerId, tokenSource.Token))
                    .ReturnsAsync(customer);

                var result = await sut.GetCustomerAsync(customer.CustomerId, tokenSource.Token).ConfigureAwait(false);

                result.Should().Be(customer);
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnNoCustomer(
               Guid randomId,
               CancellationTokenSource tokenSource,
               [Frozen] Mock<ICustomerDataStore> dataStore,
               CustomerService sut)
            {
                dataStore
                    .Setup(x => x.GetCustomerAsync(randomId, tokenSource.Token))
                    .ReturnsAsync((Interfaces.Customer)null);

                var result = await sut.GetCustomerAsync(randomId, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeNull();
            }
        }

        public class GetCustomersTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldGetAllCustomers(
                IEnumerable<Interfaces.Customer> customers,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                dataStore
                    .Setup(x => x.GetCustomersAsync(true, tokenSource.Token))
                    .ReturnsAsync(customers);

                var result = await sut.GetCustomersAsync(true, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeEquivalentTo(customers);
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldGetActiveCustomers(
                IEnumerable<Interfaces.Customer> customers,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                dataStore
                    .Setup(x => x.GetCustomersAsync(false, tokenSource.Token))
                    .ReturnsAsync(customers);

                var result = await sut.GetCustomersAsync(false, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeEquivalentTo(customers);
            }
        }

        public class UpdateCustomerTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldUpdateCustomer(
                Interfaces.Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                var result = await sut.UpdateCustomerAsync(customer, tokenSource.Token).ConfigureAwait(false);

                dataStore.Verify(x => x.UpdateCustomerAsync(customer, tokenSource.Token), Times.Once);
                result.Should().BeTrue();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldNotUpdateCustomerWithNoPrimaryAddress(
                Interfaces.Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                customer.PrimaryAddress = null;

                var result = await sut.UpdateCustomerAsync(customer, tokenSource.Token).ConfigureAwait(false);

                dataStore.Verify(x => x.UpdateCustomerAsync(customer, tokenSource.Token), Times.Never);
                result.Should().BeFalse();
            }
        }

        public class DeleteCustomerTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldDeleteCustomer(
                Guid customerId,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerDataStore> dataStore,
                CustomerService sut)
            {
                await sut.DeleteCustomerAsync(customerId, tokenSource.Token).ConfigureAwait(false);

                dataStore.Verify(x => x.DeleteCustomerAsync(customerId, tokenSource.Token), Times.Once);
            }
        }
    }
}
