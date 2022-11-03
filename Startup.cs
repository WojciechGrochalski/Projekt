
using AngularApi.MyTools;
using AngularApi.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AngularApi.DataBase;
using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using angularapi.Services;

namespace AngularApi
{
    public class Startup
    {
        private static readonly string _secret = "Superlongsupersecret!";
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //>>>>>>>>>>>>>>>Data base >>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            // var connection = @"Server=(localdb)\mssqllocaldb;Database=CashDB;Trusted_Connection=True;ConnectRetryCount=0";

            services.AddDbContext<CashDBContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("MyAzureDataBase")));

            services.AddHostedService<UpdateFileService>();
            services.AddScoped<IWebParser, MyWebParser>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IUserAuthenticate, UserAuthenticateService>();
            services.AddCors();


            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            JwtBearerOptions options(JwtBearerOptions jwtBearerOptions, string audience)
            {
                jwtBearerOptions.RequireHttpsMetadata = false;
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret)),
                    ValidateIssuer = false,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(0) //1 minute tolerance for the expiration date
                };
                if (audience == "access")
                {
                    jwtBearerOptions.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");   
                            }
                            return Task.CompletedTask;
                        }
                    };
                }
              
                return jwtBearerOptions;
            }

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtBearerOptions => options(jwtBearerOptions, "access"))
            .AddJwtBearer ("refresh",jwtBearerOptions => options(jwtBearerOptions, "refresh"));



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }
            app.UseCors(
              options => options.WithOrigins("http://localhost:4200")
                .AllowAnyMethod().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRouting();
            app.UseDefaultFiles()
               .UseStaticFiles()
               .UseWebSockets()
               .UseRouting()
               .UseAuthorization()
           .UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });


            app.UseSpa(spa =>
            {

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
