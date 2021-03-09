namespace CustomerService.Database
{
    using AutoMapper;
    using System.Linq;

    public class CustomerServiceDatabaseMapper : ICustomerServiceDatabaseMapper
    {
        private readonly IMapper Mapper = CreateMapper();

        public T Map<T>(object obj)
        {
            return this.Mapper.Map<T>(obj);
        }

        private static IMapper CreateMapper()
        {
            var configuration = new MapperConfiguration(config =>
            {
                config.CreateMap<Interfaces.Address, Address>()
                    .ReverseMap();

                config.CreateMap<Interfaces.Customer, Customer>()
                    .ForMember(x => x.Addresses, opts => opts.MapFrom(x => x.SecondaryAddresses.Prepend(x.PrimaryAddress).ToList()))
                    .AfterMap((customer, dbCustomer) =>
                    {
                        dbCustomer.Addresses.First(x => x.AddressId == customer.PrimaryAddress.AddressId).PrimaryAddress = true;
                    })
                    .ReverseMap()
                    .ForMember(x => x.PrimaryAddress, opts => opts.MapFrom(x => x.Addresses.First(y => y.PrimaryAddress)))
                    .ForMember(x => x.SecondaryAddresses, opts => opts.MapFrom(x => x.Addresses.Where(y => !y.PrimaryAddress)));
            });

            return configuration.CreateMapper();
        }
    }
}
