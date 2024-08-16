using Amazon;
using ForumCRUD.API.Data;
using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("ForumConnection");

builder.Services.AddDbContext<ForumContext>(opts =>
{
    opts.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});
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

builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ForumContext>()
                .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("177.76.128.153",
                                              "https://localhost:44476")
                          .AllowAnyHeader()
                          .AllowAnyMethod(); ;
                      });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
