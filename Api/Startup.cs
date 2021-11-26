using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            // CORS Settings
            services.AddCors(options => options.AddDefaultPolicy(policy => {
                policy.AllowAnyHeader()
                        .AllowAnyMethod()
                            .AllowCredentials()
                                    .SetIsOriginAllowed(x => true);
            }));
            
            // Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
            });

            // Cookie Settings
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x=> 
            {
                x.LoginPath="/Login.html";
                x.LogoutPath="/Logout.html";
                x.AccessDeniedPath="/accessdenied.html";
                x.Cookie.HttpOnly = false; 
                x.Cookie.Name = "TokenUserCookie";
                x.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict; 
                x.ExpireTimeSpan = TimeSpan.FromMinutes(5); 
                x.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
            
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api v1"));
            }

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseCookiePolicy();

            app.UseRouting();
            
            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
