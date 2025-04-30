
using BookingService.Data;
using BookingService.Middleware;
using BookingService.Repositories;
using BookingService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace BookingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Configuration.AddUserSecrets<Program>();

            builder.Services.AddDbContext<BookingServiceDbContext>(
                options => options.UseNpgsql(
                    builder.Configuration["ConnectionStrings:DefaultConnection"]));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

                options.AddPolicy("FrontendClient", policy =>
                {
                    policy
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))

                };
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAutoMapper(typeof(Program));

            // Hosted Services
            builder.Services.AddHostedService<BookingStatusUpdaterService>();
            builder.Services.AddHostedService<CodeCleanupBackgroundService>();

            // Services
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddTransient<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<IGoogleCloudStorageService, GoogleCloudStorageService>();
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPasswordRecoveryService, PasswordRecoveryService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IBusinessService, BusinessService>();
            builder.Services.AddScoped<IFavoritesService, FavoritesService>();
            builder.Services.AddScoped<IConfirmationCodeService, ConfirmationCodeService>();
            builder.Services.AddScoped<ICodeCleanupService, CodeCleanupService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IServiceGroupService, ServiceGroupService>();
            builder.Services.AddScoped<IServiceService, ServiceService>();
            builder.Services.AddScoped<IBookingService, BookingService.Services.BookingService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            // Repositories
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IConfirmationCodeRepository, ConfirmationCodeRepository>();
            builder.Services.AddScoped<IBusinessCategoryRespository, BusinessCategoryRepository>();
            builder.Services.AddScoped<IBusinessRepository, BusinessRepository>();
            builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IEmployeeRepository,  EmployeeRepository>();
            builder.Services.AddScoped<IServiceGroupRepository, ServiceGroupRepository>();
            builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
            builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
            builder.Services.AddScoped<IBusinessImageRepository, BusinessImageRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

            builder.Services.AddAuthentication();

            builder.Services.AddControllers()
                .AddJsonOptions(options => 
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Enter the token in the format: Bearer {token}",
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
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
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

            app.UseCors("FrontendClient");

            app.UseGlobalErrorHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
