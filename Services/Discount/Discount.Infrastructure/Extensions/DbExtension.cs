using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Security.Cryptography;

namespace Discount.Infrastructure.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            // Here would be the logic to migrate the database
            // For example, applying migrations or seeding initial data
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var config = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();
                try
                {
                    logger.LogInformation("Discount DB Migration started.");
                    ApplyMigrations(config);
                    logger.LogInformation("Discount DB Migration completed.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occured while migrating the database.");
                    throw;
                }
            }
            return host;
        }

        private static void ApplyMigrations(IConfiguration config)
        {
            var retry = 5;
            while(retry > 0)
            {
                try
                {
                    // Implement the actual migration logic here
                    using var connection = new Npgsql.NpgsqlConnection(
                        config.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();
                    using var cmd = new NpgsqlCommand()
                    {
                        Connection = connection
                    };
                    cmd.CommandText = "DROP TABLE IF EXISTS Coupon";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                    ProductName VARCHAR(500) NOT NULL,
                                                    Description TEXT,
                                                    Amount INT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Adidas Quick Force Indoor Badminton Shoes', 'Shoe Discount', 500);";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Yonex VCORE Pro 100 A Tennis Racquet (270gm, Strung)', 'Racquet Discount', 700);";
                    cmd.ExecuteNonQuery();

                    //exit if loop is successful
                    break;
                }
                catch (Exception ex)
                {
                    retry--;
                    if(retry == 0)
                    {
                        throw;
                    }
                    //wait for 2 seconds before retrying
                    Thread.Sleep(2000);
                }
            }
            
        }
    }
}
