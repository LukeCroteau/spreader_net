using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpreaderMasterService.Entities;

namespace SpreaderMasterService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SpreaderContext>(options => options.UseNpgsql(Configuration.GetConnectionString("SpreaderDatabase")));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();

            app.Run(context =>
            {
                return context.Response.WriteAsync("Currently Unimplemented");
            });

        }
    }
}
