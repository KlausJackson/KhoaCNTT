
using KhoaCNTT.API.Extensions;
using KhoaCNTT.API.Filters;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 250 * 1024 * 1024; // 200MB
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwagger();

builder.Services.AddControllers(options => {
    options.Filters.Add<ApiExceptionFilter>();
});
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



//using KhoaCNTT.Application.Interfaces.Services;
//using KhoaCNTT.Application.Services;
//using KhoaCNTT.Infrastructure.ExternalServices;
//using KhoaCNTT.Infrastructure.Identity;
//using KhoaCNTT.Infrastructure.Persistence;
//using KhoaCNTT.Infrastructure.Storage;
//using KhoaCNTT.API.Filters;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System;
//using System.Text;
//using KhoaCNTT.Application.Interfaces.Repositories;
//using KhoaCNTT.Infrastructure.Repositories;
//using KhoaCNTT.Application.Common.Utils;

//var builder = WebApplication.CreateBuilder(args);

//// 1. Cấu hình DB Context (Kết nối SQL Server)
//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// 2. Đăng ký các Service (Dependency Injection)
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

////
//// 
////

//builder.Services.AddScoped<ISchoolApiService, SchoolApiClient>();
//builder.Services.AddScoped<IAdminRepository, AdminRepository>();
//builder.Services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
//builder.Services.AddTransient<PasswordHasher>();

//// Controllers với bộ lọc API
//builder.Services.AddControllers(options =>{
//        options.Filters.Add<ApiExceptionFilter>();
//    });
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//builder.Services.AddEndpointsApiExplorer();

//// 3. Cấu hình Swagger
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Khoa CNTT API", Version = "v1" });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = "Nhập token theo định dạng: Bearer {token}",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
//            },
//            new string[] { }
//        }
//    });
//});

//// 4. Cấu hình JWT Authentication
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
//    };
//});

//// 5. Cấu hình HTTP Client để gọi API trường
//builder.Services.AddHttpClient<ISchoolApiService, SchoolApiClient>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration["SchoolApi:BaseUrl"]);
//});
////.ConfigurePrimaryHttpMessageHandler(() =>
////{
////    // BỎ QUA LỖI SSL
////    return new HttpClientHandler
////    {
////        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
////    };
////});

//// --- Dựng app ---
//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//// Thứ tự xác thực
//app.UseAuthentication(); // Xác thực (Bạn là ai?)
//app.UseAuthorization();  // Phân quyền (Bạn được làm gì?)

//app.MapControllers();

//app.Run();