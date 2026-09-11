
using Microsoft.EntityFrameworkCore;
using PersonalDigitalVault.Services.Implementations;
using PersonalDigitalVaultBackend.Controllers;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Repositories.Implementations;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Implementations;
using PersonalDigitalVaultBackend.Services.Interface;

namespace PersonalDigitalVaultBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ---------- Database ----------
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            

            // Add services to the container.

            builder.Services.AddScoped<IFolderService, FolderService>();

            // ---------- Repositories ----------
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ICredentialRepository, CredentialRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<IFolderRepository, FolderRepository>();
            

            // ---------- Services ----------
            builder.Services.AddScoped<IAuService , AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICredentialService, CredentialService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IFolderService, FolderService>();
            // Add services to the container.


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            
            
            // ---------- Swagger ----------
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Personal Digital Vault API",
                    Version = "v1"
                });

                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
