namespace CustomerServiceNS
{
    using Autofac;
    using Microsoft.EntityFrameworkCore;

    public class CustomerServiceAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<Database.CustomerServiceDbContext>();
                optionsBuilder.UseInMemoryDatabase("ModernMilkman");

                return new Database.CustomerServiceDbContext(optionsBuilder.Options);
            })
            .InstancePerLifetimeScope();

            builder.RegisterType<Database.CustomerServiceDatabaseMapper>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<Database.CustomerDataStore>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<CustomerService>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}
