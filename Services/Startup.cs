using Business.Business;
using Business.Interface;
using Data.Repositories.NaoRelacional.Context;
using Data.Repositories.NaoRelacional.Interface;
using Data.Repositories.NaoRelacional.Repository;
using Data.Repositories.Relacional.Context;
using Data.Repositories.Relacional.Interface;
using Data.Repositories.Relacional.Repository;
using Data.Repositories.XmlTexto.Context;
using Data.Repositories.XmlTexto.Interfaces;
using Data.Repositories.XmlTexto.Repository;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class Startup : IStartup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddDbContext<MobilusRelacionalContext>();
            services.AddSingleton<MobilusXmlContext>();
            services.AddSingleton<MobilusNaoRelacionalContext>();

            services.AddTransient<IProdutoRelacionalRepository, ProdutoRelacionalRepository>();
            services.AddTransient<IProdutoNaoRelacionalRepository, ProdutoNaoRelacionalRepository>();
            services.AddTransient<IProdutoXmlRepository, ProdutoXmlRepository>();

            services.AddTransient<IProdutoBusiness, ProdutoBusiness>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public void Configure(WebApplication app, IWebHostEnvironment environment)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHttpsRedirection();
            app.MapControllers();
        }
    }

    public interface IStartup
    {
        IConfiguration Configuration { get; }
        void Configure(WebApplication app, IWebHostEnvironment enviroment);
        void ConfigureServices(IServiceCollection services);
    }

    public static class StartupExtensions
    {
        public static WebApplicationBuilder UseStartup<T>(this WebApplicationBuilder webApplicationBuilder) where T : IStartup
        {
            var startup = Activator.CreateInstance(typeof(T), webApplicationBuilder.Configuration) as IStartup;
            if (startup == null)
            {
                throw new ArgumentException("Startup class is not available.");
            }

            startup.ConfigureServices(webApplicationBuilder.Services);

            var app = webApplicationBuilder.Build();

            startup.Configure(app, app.Environment);
            app.Run();

            return webApplicationBuilder;
        }
    }
}
