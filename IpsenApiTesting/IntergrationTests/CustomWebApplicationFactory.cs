using GMAPI;
using GMAPI.Models;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IpsenApiTesting
{
    public class CustomWebApplicationFactory<TStartup>: WebApplicationFactory<Startup>
    {
        /**
         * Build the testing application.
         */
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //Create a new service provider.
                var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

                services.AddDbContext<PostgresDatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryAppDb");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var appDb = scopedServices.GetRequiredService<PostgresDatabaseContext>();

                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    appDb.Database.EnsureCreated();

                    try
                    {
                        SeedData.PopulateTestData(appDb);
                    }
                    catch (Exception e) {
                        logger.LogError(e, "An error occured seeding the database with test data");
                    }
                }
            });
            
        }
    }
}
