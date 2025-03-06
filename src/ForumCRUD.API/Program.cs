using Amazon;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using ForumCRUD.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using System.Text;
using System.Threading;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();});
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration["DB_CONNECTION_STRING"];

// Add the database connection semaphore - limits concurrent DB operations
// Set to 4 to stay under the MySQL user connection limit of 5 (leaving 1 free for admin operations)
builder.Services.AddSingleton<SemaphoreSlim>(new SemaphoreSlim(4, 4));

// Create a MySqlConnectionStringBuilder to set connection pool limits
var mySqlConnectionStringBuilder = new MySqlConnector.MySqlConnectionStringBuilder(connectionString)
{
    MaximumPoolSize = 5, // Set slightly higher than SemaphoreSlim count
    MinimumPoolSize = 0,
    ConnectionLifeTime = 60, // 1 minute
    ConnectionTimeout = 30, // 30 seconds
    DefaultCommandTimeout = 30 // 30 seconds
};

// Update the connection string with pool settings
connectionString = mySqlConnectionStringBuilder.ConnectionString;

// Register MySQL interceptors
builder.Services.AddSingleton<MySqlRetryConnectionInterceptor>();

// Configure the DbContext with our custom execution strategy
builder.Services.AddDbContext<ForumContext>((serviceProvider, opts) =>
{
    opts.UseMySql(connectionString, 
        ServerVersion.AutoDetect(connectionString),
        options => options
            // Use our custom retry execution strategy instead of the default one
            .ExecutionStrategy(dependencies => new MySqlRetryExecutionStrategy(
                dependencies.CurrentContext.Context,
                maxRetryCount: 9999,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                serviceProvider.GetRequiredService<ILogger<MySqlRetryExecutionStrategy>>()))
            .CommandTimeout(30)
            .MigrationsAssembly("ForumCRUD.API")
            .MaxBatchSize(10) // Limit batch size
            .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)); // Split complex queries

    // Set connection pool limits to align with SemaphoreSlim
    opts.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    opts.EnableDetailedErrors(builder.Environment.IsDevelopment());
    
    // Add custom interceptors
    var logger = serviceProvider.GetRequiredService<ILogger<MySqlRetryConnectionInterceptor>>();
    opts.AddInterceptors(new MySqlRetryConnectionInterceptor(logger));
    
    // Set execution strategy
    opts.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
}, ServiceLifetime.Scoped);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ForumAPI",
        Version = "v1",
        Description = "Projeto de API para um Fórum de Discussão",
        Contact = new OpenApiContact
        {
            Name = "Vitor Carvalho",
            Url = new System.Uri("https://github.com/VitorCCarvalho"),
            Email = "vitorcoscarvalho@gmail.com"
        }
    });
    
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes("98ahyd9asn8dhFN0SDFNASDLKFNDSF0SD8uyr9esj8fdso0i")),
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ForumContext>()
                .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<DatabaseQueueService>();

builder.Configuration.AddUserSecrets<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
