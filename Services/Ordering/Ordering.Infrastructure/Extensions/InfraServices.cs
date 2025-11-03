
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;

namespace Ordering.Infrastructure.Extensions
{
    public static class InfraServices
    {
        public static IServiceCollection AddInfraServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            // Here you can add infrastructure related services to the DI container
            // For example, registering the DbContext, repositories, etc.
            // services.AddDbContext<OrderContext>(options =>
            //     options.UseSqlServer("YourConnectionString"));
            // services.AddScoped<IOrderRepository, OrderRepository>();
            serviceCollection.AddDbContext<OrderContext>(options => options.UseSqlServer(
                configuration.GetConnectionString("OrderingConnectionString")
                ));
            serviceCollection.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
            return serviceCollection;
        }
    }
}
