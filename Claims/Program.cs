using System.Reflection;
using System.Text.Json.Serialization;
using Claims.Auditing;
using Claims.Data.AuditData;
using Claims.Data.ClaimData;
using Claims.Messaging;
using Claims.Repositories;
using Claims.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllers()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AuditContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<ClaimsContext>(
    options =>
    {
        var client = new MongoClient(builder.Configuration.GetConnectionString("MongoDb"));
        var database = client.GetDatabase(builder.Configuration["MongoDb:DatabaseName"]);
        options.UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName);
    }
);

var settings = new ServiceBusSettings();
builder.Configuration.GetSection("AzureServiceBus").Bind(settings);
builder.Services.AddSingleton(settings);

builder.Services.AddSingleton<IAuditHandler, AuditHandler>();
builder.Services.AddScoped<IClaimAuditRepository, ClaimAuditRepository>();
builder.Services.AddScoped<ICoverAuditRepository, CoverAuditRepository>();
builder.Services.AddScoped<IClaimRepository, ClaimRepository>();
builder.Services.AddScoped<ICoverRepository, CoverRepository>();
builder.Services.AddScoped<IClaimService, ClaimService>();
builder.Services.AddScoped<ICoverService, CoverService>();
builder.Services.AddScoped<IComputePremiumService, ComputePremiumService>();
builder.Services.AddScoped<IBusSender, BusSender>();
builder.Services.AddSingleton<IBusConsumer, BusConsumer>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Claims and Covers API",
        Description = "A Web API for managing Claims and Covers",
        Contact = new OpenApiContact
        {
            Name = "Erik Aandahl",
            Email = "erik@aandahl.net"
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
    context.Database.Migrate();
}

var bus = app.Services.GetRequiredService<IBusConsumer>();
bus.RegisterOnMessageHandlerAndReceiveMessages().GetAwaiter().GetResult();


app.Run();

public partial class Program { }
