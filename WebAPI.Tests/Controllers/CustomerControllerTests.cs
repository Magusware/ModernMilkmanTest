namespace WebAPI.Tests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AutoFixture.Xunit2;
    using CustomerServiceNS.Interfaces;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using Microsoft.AspNetCore.Mvc;
    using Moq;
    using WebAPI.Controllers;
    using Xunit;

    public class CustomerControllerTests
    {
        public class GetAsyncTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldReturnBadRequestWithEmptyGuid(
                CancellationTokenSource tokenSource,
                CustomerController sut)
            {
                var result = await sut.GetAsync(Guid.Empty, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<BadRequestResult>();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnNotFound(
                Guid customerId,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.GetCustomerAsync(customerId, tokenSource.Token)).ReturnsAsync((Customer)null);
                var result = await sut.GetAsync(customerId, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<NotFoundResult>();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnOkWithCustomer(
                Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.GetCustomerAsync(customer.CustomerId, tokenSource.Token)).ReturnsAsync(customer);
                var result = await sut.GetAsync(customer.CustomerId, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(customer);
            }
        }

        public class GetAllAsyncTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldReturnAll(
                IEnumerable<Customer> customers,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.GetCustomersAsync(true, tokenSource.Token)).ReturnsAsync(customers);
                var result = await sut.GetAllAsync(tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().Be(customers);
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnNothing(
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.GetCustomersAsync(true, tokenSource.Token)).ReturnsAsync(Array.Empty<Customer>());
                var result = await sut.GetAllAsync(tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().BeAssignableTo<IEnumerable<Customer>>()
                    .Which.Should().HaveCount(0);
            }
        }

        public class GetActiveAsyncTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldReturnAll(
                IEnumerable<Customer> customers,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.GetCustomersAsync(false, tokenSource.Token)).ReturnsAsync(customers);
                var result = await sut.GetActiveAsync(tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().Be(customers);
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnNothing(
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.GetCustomersAsync(false, tokenSource.Token)).ReturnsAsync(Array.Empty<Customer>());
                var result = await sut.GetActiveAsync(tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().BeAssignableTo<IEnumerable<Customer>>()
                    .Which.Should().HaveCount(0);
            }
        }

        public class PutAsyncTests
        {
            [Theory]
            [AutoDomainData]
            public async void ShouldReturnBadRequestWithInvalidCustomer(
                Customer customer,
                CancellationTokenSource tokenSource,
                CustomerController sut)
            {
                customer.Title = "VeryLongTitleWhichWillExceedTheValidityCheck";
                var result = await sut.PutAsync(customer, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<BadRequestResult>();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnConflictIfFailedToAdd(
                Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.AddCustomerAsync(customer, tokenSource.Token)).ReturnsAsync(false);
                var result = await sut.PutAsync(customer, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<ConflictResult>();
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldReturnOkWithCustomerObject(
                Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                service.Setup(x => x.AddCustomerAsync(customer, tokenSource.Token)).ReturnsAsync(true);
                var result = await sut.PutAsync(customer, tokenSource.Token).ConfigureAwait(false);

                result.Should().BeOfType<OkObjectResult>()
                    .Which.Value.Should().Be(customer);
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldSetDefaultCountryOnPrimaryAddress(
                Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                customer.PrimaryAddress.Country = null;

                service.Setup(x => x.AddCustomerAsync(customer, tokenSource.Token)).ReturnsAsync(true);
                var result = await sut.PutAsync(customer, tokenSource.Token).ConfigureAwait(false);

                customer.PrimaryAddress.Country.Should().Be(sut.DefaultCountry);
            }

            [Theory]
            [AutoDomainData]
            public async void ShouldSetDefaultCountryOnSecondaryAddresses(
                Customer customer,
                CancellationTokenSource tokenSource,
                [Frozen] Mock<ICustomerService> service,
                CustomerController sut)
            {
                foreach (var address in customer.SecondaryAddresses)
                {
                    address.Country = null;
                }

                service.Setup(x => x.AddCustomerAsync(customer, tokenSource.Token)).ReturnsAsync(true);
                var result = await sut.PutAsync(customer, tokenSource.Token).ConfigureAwait(false);

                using (new AssertionScope())
                {
                    foreach (var address in customer.SecondaryAddresses)
                    {
                        address.Country.Should().Be(sut.DefaultCountry);
                    }
                }
            }
        }
    
        /* Add more unit tests */
    }
}
