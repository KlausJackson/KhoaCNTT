
using KhoaCNTT.Application.Common.Utils;
using KhoaCNTT.Application.Interfaces.Repositories;
using KhoaCNTT.Application.Interfaces.Repositories.INewsRepositories;
using KhoaCNTT.Application.Interfaces.Services;
using KhoaCNTT.Application.Interfaces.Services.INewsServices;
using KhoaCNTT.Application.Services;
using KhoaCNTT.Infrastructure.ExternalServices;
using KhoaCNTT.Infrastructure.Identity;
using KhoaCNTT.Infrastructure.Persistence;
using KhoaCNTT.Infrastructure.Repositories;
using KhoaCNTT.Infrastructure.Repositories.News;
using KhoaCNTT.Infrastructure.Repositories.File;
using KhoaCNTT.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace KhoaCNTT.API.Utils
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // 2. Services & Repositories
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ISchoolApiService, SchoolApiClient>();

            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ILecturerRepository, LecturerRepository>();
            services.AddScoped<ILecturerService, LecturerService>();

            // 3. News Repositories
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<INewsResourceRepository, NewsResourceRepository>();
            services.AddScoped<INewsRequestRepository, NewsRequestRepository>();
            services.AddScoped<INewsApprovalRepository, NewsApprovalRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IFileRequestRepository, FileRequestRepository>();
            services.AddScoped<IFileResourceRepository, FileResourceRepository>();
            services.AddScoped<IFileApprovalRepository, FileApprovalRepository>();
            services.AddScoped<IFileService, FileService>();


            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddTransient<PasswordHasher>();

            // 3. AutoMapper & Http
            services.AddAutoMapper(typeof(KhoaCNTT.Application.Common.Utils.AutoMapperProfile));
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddHttpClient<ISchoolApiService, SchoolApiClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["SchoolApi:BaseUrl"]);
            });

            // 4. JWT Auth
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Khoa CNTT API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Nhập: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] { }
                    }
                });
            });
            return services;
        }
    }
}