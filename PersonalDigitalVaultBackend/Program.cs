
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalDigitalVault.Services.Implementations;
using PersonalDigitalVaultBackend.Controllers;
using PersonalDigitalVaultBackend.Data;
using PersonalDigitalVaultBackend.Middleware;
using PersonalDigitalVaultBackend.Repositories.Implementations;
using PersonalDigitalVaultBackend.Repositories.Interfaces;
using PersonalDigitalVaultBackend.Services.Implementations;
using PersonalDigitalVaultBackend.Services.Interface;
using System.Text;

namespace PersonalDigitalVaultBackend
{
    public class Program
    {
        public static async Task Main(string[] args)
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
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<ISharedLinkRepository, SharedLinkRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();


            // ---------- Services ----------
            builder.Services.AddScoped<IAuService , AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICredentialService, CredentialService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IFolderService, FolderService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<ISharingService, SharingService>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
            builder.Services.AddSingleton<IFileStorageService, FileStorageService>();

            // Add services to the container.


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // ---------- CORS (Angular dev server) ----------
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:4200" };

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AngularClient", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ---------- JWT Authentication ----------
            var jwtKey = builder.Configuration["Jwt:Key"]!;
            var encryptionKey = builder.Configuration["Encryption:Key"]!;

            // Security review finding: both secrets used to ship as unchanged
            // placeholder text ("CHANGE_THIS_TO_..."), which is a full auth bypass
            // (Jwt:Key lets anyone forge a valid token for any user) and a full
            // data-confidentiality bypass (Encryption:Key decrypts every file and
            // credential) if ever deployed as-is. Failing fast here means that
            // mistake can never happen silently again.
            if (jwtKey.StartsWith("CHANGE_THIS", StringComparison.OrdinalIgnoreCase) || jwtKey.Length < 32)
            {
                throw new InvalidOperationException(
                    "Jwt:Key in appsettings.json must be changed to a real random secret at least 32 characters long before running this app.");
            }
            if (encryptionKey.StartsWith("CHANGE_THIS", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Encryption:Key in appsettings.json must be changed to a real random base64-encoded 32-byte key before running this app.");
            }

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            builder.Services.AddAuthorization();

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

            // ---------- Seed the Administrator account & ensure DB exists ----------
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                await AdminSeeder.SeedAsync(context, config);
            }



            // ---------- Middleware pipeline ----------
            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // No app.UseHttpsRedirection() here - this project runs on plain HTTP
            // (http://localhost:7000) with no HTTPS certificate/port configured, so
            // that middleware would try to redirect to an HTTPS port that doesn't
            // exist rather than doing anything useful.
            var indexHtmlNoCacheOptions = new StaticFileOptions
            {
                // Hashed chunk/css filenames (e.g. chunk-XXXX.js) change on every
                // build, so they're safe to cache indefinitely. index.html itself
                // never changes name, so without this it can get stuck in the
                // browser's cache pointing at old (now-deleted) chunk filenames from
                // a previous build - looking exactly like new code "isn't showing
                // up" even after the server has been redeployed with fresh files.
                OnPrepareResponse = ctx =>
                {
                    if (ctx.File.Name.Equals("index.html", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
                    }
                }
            };
            app.UseStaticFiles(indexHtmlNoCacheOptions); // serves wwwroot - used when the Angular build output is copied here (Method 2 integration)
            app.UseCors("AngularClient");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Fallback to index.html for Angular client-side routing when the SPA is hosted from wwwroot.
            app.MapFallbackToFile("index.html", indexHtmlNoCacheOptions);

            app.Run();

        }
    }
}
