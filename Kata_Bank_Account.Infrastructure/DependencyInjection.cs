using Kata_Bank_Account.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Kata_Bank_Account.Infrastructure.Persistence;

namespace Kata_Bank_Account.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DockerDb")));
                //options.UseInMemoryDatabase("Kata_Bank_Account"));

            services.AddScoped<IAppDbContext, AppDbContext>();
        }
    }
}
