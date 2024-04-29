using iEvent.Domain;
using Microsoft.EntityFrameworkCore;

namespace iEvent.Infastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            var builder = WebApplication.CreateBuilder();
            ConfigurationManager configuration = builder.Configuration;
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Connstr")));
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<ApplicationDbContext>());
            services.AddTransient<IManageImage, ManageImage>();

        }
    }
}
