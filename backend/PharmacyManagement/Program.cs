using PharmacyManagement.Models;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Middlewares;
using PharmacyManagement.DTOs.Auth;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PharmacyManagement.share;

using PharmacyManagement.Services.Interfaces;
using PharmacyManagement.Services.Implements;
using PharmacyManagement.Services.BatchSelection;

using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Repositories.Implements;
using FluentValidation;
using PharmacyManagement.Validators.BusinessRule;
using PharmacyManagement.Validators.FluentValidation.User;
using PharmacyManagement.Validators.PermissionHandle;
using PharmacyManagement.Handlers;
using Microsoft.AspNetCore.Authorization;

namespace PharmacyManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<PharmacySystemDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("PharSystemConnection")));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtConfig"));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "http://localhost:5000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;

                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings =
                    builder.Configuration
                    .GetSection("JwtConfig")
                    .Get<JwtSetting>();

                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer = jwtSettings.Issuer,

                        ValidAudience = jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

                        ClockSkew = TimeSpan.Zero
                    };


                options.Events = new JwtBearerEvents
                {
                    // Case: token không hợp lệ
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";

                            var response = new ApiResponse<object>
                            {
                                EC = -998,
                                StatusCode = 401,
                                EM = "Access token is invalid.",
                                DT = null
                            };

                            return context.Response.WriteAsJsonAsync(response);
                        }

                        return Task.CompletedTask;
                    },
                    // Case: token hết hạn
                    OnAuthenticationFailed = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            if (context.Exception is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                context.Response.ContentType = "application/json";

                                var response = new ApiResponse<object>
                                {
                                    EC = -999,
                                    StatusCode = 401,
                                    EM = "Access token has expired.",
                                    DT = null
                                };

                                return context.Response.WriteAsJsonAsync(response);
                            }
                        }

                        return Task.CompletedTask;
                    },

                    OnForbidden = context =>
                    {
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";

                            var response = new ApiResponse<object>
                            {
                                StatusCode = 403,
                                EM = "You do not have permission to access this resource.",
                                DT = null,
                                EC= -1
                            };

                            return context.Response.WriteAsJsonAsync(response);
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();


            // Repository
            builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
            builder.Services.AddScoped<ICustomerTypeRepository, CustomerTypeRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IUnitRepository, UnitRepository>();
            builder.Services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            builder.Services.AddScoped<IMedicineCategoryRepository, MedicineCategoryRepository>();
            builder.Services.AddScoped<IBranchRepository, BranchRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
            builder.Services.AddScoped<IUnitConversionRepository, UnitConversionRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IGoodsReceiptRepository, GoodsReceiptRepository>();
            builder.Services.AddScoped<IStockTakeRepository, StockTakeRepository>();
            builder.Services.AddScoped<IStockAdjustmentRepository, StockAdjustmentRepository>();
            builder.Services.AddScoped<IDestroyReceiptRepository, DestroyReceiptRepository>();

            // Service
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPermissionService, PermissionService>();
            builder.Services.AddScoped<ICustomerTypeService, CustomerTypeService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUnitService, UnitService>();
            builder.Services.AddScoped<IManufacturerService, ManufacturerService>();
            builder.Services.AddScoped<IMedicineCategoryService, MedicineCategoryService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IMedicineService, MedicineService>();
            builder.Services.AddScoped<IUnitConversionService, UnitConversionService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<IGoodsReceiptService, GoodsReceiptService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IStockTakeService, StockTakeService>();
            builder.Services.AddScoped<IStockAdjustmentService, StockAdjustmentService>();
            builder.Services.AddScoped<IDestroyReceiptService, DestroyReceiptService>();

            // Batch selection
            builder.Services.AddScoped<FefoBatchSelectionStrategy>();
            builder.Services.AddScoped<ManualBatchSelectionStrategy>();
            builder.Services.AddScoped<BatchSelectionStrategyFactory>();

            // Authorization
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

            // Validators
            builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
            builder.Services.AddScoped<UserBusinessValidator>();
            builder.Services.AddScoped<CustomerTypeBusinessValidator>();
            builder.Services.AddScoped<RoleBusinessValidator>();
            builder.Services.AddScoped<UnitBusinessValidator>();
            builder.Services.AddScoped<ManufacturerBusinessValidator>();
            builder.Services.AddScoped<MedicineCategoryBusinessValidator>();
            builder.Services.AddScoped<BranchBusinessValidator>();
            builder.Services.AddScoped<CustomerBusinessValidator>();
            builder.Services.AddScoped<SupplierBusinessValidator>();
            builder.Services.AddScoped<WarehouseBusinessValidator>();
            builder.Services.AddScoped<MedicineBusinessValidator>();
            builder.Services.AddScoped<UnitConversionBusinessValidator>();
            builder.Services.AddScoped<InvoiceBusinessValidator>();
            builder.Services.AddScoped<InvoiceItemBusinessValidator>();
            builder.Services.AddScoped<GoodsReceiptBusinessValidator>();
            builder.Services.AddScoped<GoodsReceiptItemUpdateHandler>();
            builder.Services.AddScoped<StockTakeBusinessValidator>();
            builder.Services.AddScoped<StockAdjustmentBusinessValidator>();
            builder.Services.AddScoped<DestroyReceiptBusinessValidator>();

            builder.Services.AddControllers();


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseCors("ReactPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            if (app.Environment.IsEnvironment("Docker"))
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PharmacySystemDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
