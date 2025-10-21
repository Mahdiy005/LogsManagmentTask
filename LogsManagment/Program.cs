using FluentValidation;
using FluentValidation.AspNetCore;
using LogsManagment.Core.Behavoires;
using LogsManagment.Core.CustomMiddlewares;
using LogsManagment.Core.Features.Logs.Commands.Validators;
using LogsManagment.Core.Hubs;
using LogsManagment.Data.Entities;
using LogsManagment.Data.Seeds;
using LogsManagment.Services.Implementations;
using LogsManagment.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

namespace LogsManagment
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<Data.AppContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")).UseLazyLoadingProxies();
            });

            // Fluent Validation
            //  ”Ã· ﬂ· «·‹ Validators «·„ÊÃÊœ… ›Ì «·„‘—Ê⁄ (for DI)
            builder.Services.AddValidatorsFromAssemblyContaining<AddLogValidator>();
            // Pipeline · ‘€Ì· «·‹ validation  ·ﬁ«∆Ì« œ«Œ· MediatR
            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavoirs<,>));

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5500")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Localization
            #region Localization
            builder.Services.AddControllersWithViews();
            builder.Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
                {
                        new CultureInfo("en-US"),
                        new CultureInfo("de-DE"),
                        new CultureInfo("fr-FR"),
                        new CultureInfo("ar-EG")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            #endregion


            // Identity
            #region Identity Registration
            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<Data.AppContext>()
                .AddDefaultTokenProviders();
            #endregion
            // Media
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            // SignalR Hubs
            builder.Services.AddSignalR();
            // IAuth service
            builder.Services.AddScoped<IAuthService, AuthService>();

            #region Configure Jwt
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKeyForJWTTokenGeneration2025ThisIsAVeryStrongSecretKeyForJWT!")),
                    ClockSkew = TimeSpan.Zero

                };

                //  œÌ √Â„ ŒÿÊ… ⁄‘«‰ SignalR Ìﬁœ— Ì«Œœ «·‹ token „‰ «·‹ query string
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our SignalR hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/notifications"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };

            });
            #endregion

            // Configure controllers and FluentValidation integration for MVC model binding
            builder.Services.AddControllers()
                .AddFluentValidation(fv =>
                {
                    // register validators from the assembly (MVC integration)
                    fv.RegisterValidatorsFromAssemblyContaining<AddLogValidator>();
                    // make FluentValidation authoritative (disable DataAnnotations if you want)
                    fv.DisableDataAnnotationsValidation = true;
                });

            // Optional convenience registrations (auto validation + clientside adapters)
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // Seed Roles
            #region Seed Role (Admin, User)
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                RolesSeed.Seed(roleManager, userManager);
            }
            #endregion

            app.UseHttpsRedirection();

            // Use CORS before authentication/authorization
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();
            // 3?-  ›⁄Ì· Localization
            var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);


            app.MapControllers();
            // Map SignalR Hubs
            app.MapHub<NotificationHub>("/hubs/notifications");

            app.Run();
        }
    }
}
