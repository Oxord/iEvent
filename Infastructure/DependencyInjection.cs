using iEvent.Controllers;
using iEvent.Domain;
using iEvent.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace iEvent.Infastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            var builder = WebApplication.CreateBuilder();
            ConfigurationManager configuration = builder.Configuration;
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Connstr")));
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IProblemCommentRepository, ProblemCommentRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IMapOfEventRepository, MapOfEventRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<ApplicationDbContext>());
            services.AddTransient<IManageImage, ManageImage>();
            services.AddScoped<ICompanyInfoService, CompanyInfoService>();

        }
    }
}
