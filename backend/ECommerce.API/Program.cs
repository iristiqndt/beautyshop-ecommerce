using ECommerce.Application.Interfaces;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ECommerce API", 
        Version = "v1",
        Description = "E-Commerce Cosmetics Web API with Clean Architecture"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
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

// Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? Environment.GetEnvironmentVariable("DATABASE_PUBLIC_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // PostgreSQL (Railway or Supabase)
    var postgresConnectionString = databaseUrl.Replace("postgres://", "").Replace("postgresql://", "");
    var parts = postgresConnectionString.Split('@');
    var userPass = parts[0].Split(':');
    var hostDbParts = parts[1].Split('/');
    var hostPort = hostDbParts[0].Split(':');
    
    // Check if it's Supabase (requires SSL)
    var isSupabase = hostPort[0].Contains("supabase.co");
    var sslMode = isSupabase ? "Require" : "Prefer";
    
    var connString = $"Host={hostPort[0]};Port={hostPort[1]};Database={hostDbParts[1]};Username={userPass[0]};Password={userPass[1]};SSL Mode={sslMode};Trust Server Certificate=true;Pooling=true;Timeout=30;CommandTimeout=30";
    
    builder.Services.AddDbContext<ECommerceDbContext>(options =>
        options.UseNpgsql(connString,
            b => b.MigrationsAssembly("ECommerce.Infrastructure")));
}
else
{
    // Local SQL Server
    builder.Services.AddDbContext<ECommerceDbContext>(options =>
        options.UseSqlServer(connectionString,
            b => b.MigrationsAssembly("ECommerce.Infrastructure")));
}

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ECommerceAPI",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ECommerceClient",
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Dependency Injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<PayPalPaymentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint for Railway
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.MapControllers();

// Database migration and seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ECommerceDbContext>();
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed data
        await SeedData(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while setting up the database.");
        // Don't stop the app - allow it to start even if database fails
    }
}

app.Run();

static async Task SeedData(ECommerceDbContext context)
{
    // Seed Roles
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new ECommerce.Domain.Entities.Role { Name = "Admin", Description = "Administrator" },
            new ECommerce.Domain.Entities.Role { Name = "User", Description = "Regular User" }
        );
        await context.SaveChangesAsync();
    }

    // Seed Admin User
    if (!context.Users.Any())
    {
        var adminRole = context.Roles.First(r => r.Name == "Admin");
        context.Users.Add(new ECommerce.Domain.Entities.User
        {
            Email = "admin@ecommerce.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tien2004@"),
            FullName = "System Administrator",
            RoleId = adminRole.Id,
            EmailConfirmed = true
        });
        await context.SaveChangesAsync();
    }
    else
    {
        // Update existing admin password
        var admin = context.Users.FirstOrDefault(u => u.Email == "admin@ecommerce.com");
        if (admin != null)
        {
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tien2004@");
            context.Users.Update(admin);
            await context.SaveChangesAsync();
        }
    }

    // Seed Categories
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new ECommerce.Domain.Entities.Category 
            { 
                Name = "Skincare", 
                Description = "Face and body skincare products",
                Slug = "skincare"
            },
            new ECommerce.Domain.Entities.Category 
            { 
                Name = "Makeup", 
                Description = "Cosmetics and makeup products",
                Slug = "makeup"
            },
            new ECommerce.Domain.Entities.Category 
            { 
                Name = "Haircare", 
                Description = "Hair treatment and styling products",
                Slug = "haircare"
            },
            new ECommerce.Domain.Entities.Category 
            { 
                Name = "Fragrance", 
                Description = "Perfumes and body sprays",
                Slug = "fragrance"
            }
        );
        await context.SaveChangesAsync();
    }

    // Seed Sample Products
    if (!context.Products.Any())
    {
        var skincareCategory = context.Categories.First(c => c.Slug == "skincare");
        var makeupCategory = context.Categories.First(c => c.Slug == "makeup");

        context.Products.AddRange(
            new ECommerce.Domain.Entities.Product
            {
                Name = "Hydrating Face Cream",
                Description = "Moisturizing cream for all skin types",
                Price = 29.99m,
                StockQuantity = 100,
                CategoryId = skincareCategory.Id,
                Slug = "hydrating-face-cream",
                Brand = "GlowSkin",
                IsFeatured = true
            },
            new ECommerce.Domain.Entities.Product
            {
                Name = "Vitamin C Serum",
                Description = "Brightening serum with vitamin C",
                Price = 39.99m,
                StockQuantity = 75,
                CategoryId = skincareCategory.Id,
                Slug = "vitamin-c-serum",
                Brand = "RadiantLab",
                IsFeatured = true
            },
            new ECommerce.Domain.Entities.Product
            {
                Name = "Matte Lipstick Set",
                Description = "Long-lasting matte lipstick collection",
                Price = 24.99m,
                StockQuantity = 50,
                CategoryId = makeupCategory.Id,
                Slug = "matte-lipstick-set",
                Brand = "ColorPop",
                IsFeatured = false
            }
        );
        await context.SaveChangesAsync();
    }
}
