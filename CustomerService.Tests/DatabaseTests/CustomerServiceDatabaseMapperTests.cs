namespace CustomerServiceNS.Tests.DatabaseTests
{
    using System;
    using System.Linq;
    using CustomerServiceNS.Database;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Xunit;

    public class CustomerServiceDatabaseMapperTests
    {
        public class MapAddressTests
        {
            [Theory]
            [AutoDomainData]
            public void ShouldMapInterfaceAddressToDatabaseAddress(
                Interfaces.Address address,
                CustomerServiceDatabaseMapper sut)
            {
                var result = sut.Map<Database.Address>(address);
                result.Should().BeEquivalentTo(address, opts => opts.ExcludingMissingMembers());
            }

            [Theory]
            [AutoDomainData]
            public void ShouldMapDatabaseAddressToInterfaceAddress(
                Address address,
                CustomerServiceDatabaseMapper sut)
            {
                var result = sut.Map<Interfaces.Address>(address);
                result.Should().BeEquivalentTo(address, opts => opts.ExcludingMissingMembers());
            }
        }

        public class MapCustomerTests
        {
            [Theory]
            [AutoDomainData]
            public void ShouldMapInterfaceCustomerToDatabaseCustomer(
                Interfaces.Customer customer,
                CustomerServiceDatabaseMapper sut)
            {
                var result = sut.Map<Database.Customer>(customer);

                using (new AssertionScope())
                {
                    result.Should().BeEquivalentTo(customer, opts => opts.ExcludingMissingMembers());
                    foreach (var address in customer.SecondaryAddresses)
                    {
                        result.Addresses.Should().ContainEquivalentOf(address, opts => opts.ExcludingMissingMembers());
                    }

                    var primaryDbAddress = result.Addresses.Where(x => x.IsPrimary);
                    primaryDbAddress.Should().HaveCount(1);
                    primaryDbAddress.First().Should().BeEquivalentTo(customer.PrimaryAddress, opts => opts.ExcludingMissingMembers());
                }
            }

            [Theory]
            [AutoDomainData]
            public void ShouldMapInterfaceCustomerToDatabaseCustomerWithOnlyPrimaryAddress_NullSecondary(
                Interfaces.Customer customer,
                CustomerServiceDatabaseMapper sut)
            {
                customer.SecondaryAddresses = null;

                var result = sut.Map<Database.Customer>(customer);

                using (new AssertionScope())
                {
                    result.Should().BeEquivalentTo(customer, opts => opts.ExcludingMissingMembers());
                    result.Addresses.Should().HaveCount(1);
                    result.Addresses.First().Should().BeEquivalentTo(customer.PrimaryAddress, opts => opts.ExcludingMissingMembers());
                    result.Addresses.First().IsPrimary.Should().BeTrue();
                }
            }

            [Theory]
            [AutoDomainData]
            public void ShouldMapInterfaceCustomerToDatabaseCustomerWithOnlyPrimaryAddress_EmptySecondary(
                Interfaces.Customer customer,
                CustomerServiceDatabaseMapper sut)
            {
                customer.SecondaryAddresses = Array.Empty<Interfaces.Address>();

                var result = sut.Map<Database.Customer>(customer);

                using (new AssertionScope())
                {
                    result.Should().BeEquivalentTo(customer, opts => opts.ExcludingMissingMembers());
                    result.Addresses.Should().HaveCount(1);
                    result.Addresses.First().Should().BeEquivalentTo(customer.PrimaryAddress, opts => opts.ExcludingMissingMembers());
                    result.Addresses.First().IsPrimary.Should().BeTrue();
                }
            }

            [Theory]
            [AutoDomainData]
            public void ShouldMapDatabaseCustomerToInterfaceCustomer(
                Customer customer,
                CustomerServiceDatabaseMapper sut)
            {
                var result = sut.Map<Interfaces.Customer>(customer);

                using (new AssertionScope())
                {
                    result.Should().BeEquivalentTo(customer, opts => opts.Excluding(x => x.Addresses));

                    result.PrimaryAddress.Should().BeEquivalentTo(
                        customer.Addresses.First(x => x.IsPrimary),
                        opts => opts.ExcludingMissingMembers());

                    result.SecondaryAddresses.Should().BeEquivalentTo(
                        customer.Addresses.Where(x => !x.IsPrimary).ToArray(),
                        opts => opts.ExcludingMissingMembers());
                }
            }
        }
    }
}
