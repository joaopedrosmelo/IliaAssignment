using IdentityServer4.AccessTokenValidation;
using IliaAssignment.Data;
using IliaAssignment.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IliaAssignment
{
    public class Startup
    {
        public static IConfigurationRoot configuration;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDBContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            var identitySettingsSection = Configuration.GetSection("SMTP");
            services.Configure<SMTP>(identitySettingsSection);
            services.AddAutoMapper(typeof(Startup));

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options => {
                    options.Authority = Configuration["IliaAssisgnmentIdentityServer"];
                    options.ApiName = "app.api.iliaassignment";
                });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
