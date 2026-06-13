using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microservicios.Atracciones.Billing.DataAccess;
using Microservicios.Atracciones.Billing.Business;
using Microservicios.Atracciones.Billing.DataManagement;
using MassTransit;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Microservicios.Atracciones.Billing.Business.Consumers;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Configurar licencia de QuestPDF
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

builder.Services.AddHttpContextAccessor();

// 1. CONFIGURACIÓN DE CAPAS
builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBusinessServices();
builder.Services.AddDataManagementServices();

// Configurar MassTransit con RabbitMQ y Registrar Consumidor
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BookingCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitHost = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var rabbitUser = builder.Configuration["RabbitMQ:Username"] ?? "guest";
        var rabbitPass = builder.Configuration["RabbitMQ:Password"] ?? "guest";

        cfg.Host(rabbitHost, "/", h =>
        {
            h.Username(rabbitUser);
            h.Password(rabbitPass);
        });

        cfg.ReceiveEndpoint("booking-created-billing", e =>
        {
            e.ConfigureConsumer<BookingCreatedConsumer>(context);
        });
    });
});

// Configurar OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("Microservicios.Atracciones.Billing")
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("BillingService"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

// 2. CONFIGURACIÓN API & CORS
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new RoutePrefixConvention());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();

// 3. SWAGGER
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Billing Microservice API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// 4. JWT AUTHENTICATION
var jwtKey = builder.Configuration["Jwt:Key"] ?? "BillingService_Super_Secret_Key_2026";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "BillingService";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "BillingServiceUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => 
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Billing API v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public class RoutePrefixConvention : Microsoft.AspNetCore.Mvc.ApplicationModels.IApplicationModelConvention
{
    public void Apply(Microsoft.AspNetCore.Mvc.ApplicationModels.ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    var currentTemplate = selector.AttributeRouteModel.Template;
                    if (currentTemplate != null && currentTemplate.StartsWith("api/v1/"))
                    {
                        selector.AttributeRouteModel.Template = currentTemplate["api/v1/".Length..];
                    }
                }
            }
        }
    }
}
