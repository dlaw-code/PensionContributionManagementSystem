using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PensionContributionManagementSystem.Core.Abstractions;
using PensionContributionManagementSystem.Infrastructure;
using PensionContributionManagementSystem.Infrastructure.Contexts;
using PensionContributionManagementSystem.Infrastructure.Repositories;
using PensionContributionManagementSystem.Domain.Entities;
using PensionContributionManagementSystem.Core.Services;
using Hangfire;
using FluentValidation;
using FluentValidation.AspNetCore;
using PensionContributionManagementSystem.Core.Dtos;

namespace PensionContributionManagementSystem.Api.Extensions;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Fast api",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });


        
        services.AddControllers();

        services.AddFluentValidationAutoValidation(); 
        services.AddFluentValidationClientsideAdapters(); 

        
        services.AddValidatorsFromAssemblyContaining<AddEmployerDtoValidator>();

        services.AddHangfire(config =>
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

        services.AddHangfireServer();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                optionsBuilder =>
                {
                    optionsBuilder.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                }));


        services.AddIdentity<Member, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var key = Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Key").Value);
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false
            };
        });

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBenefitService, BenefitService>();
        services.AddScoped<IContributionService, ContributionService>();
        services.AddScoped<IMemberManagementService, MemberManagementService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IEmployerService, EmployerService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<BackgroundJobService>();

    }
}