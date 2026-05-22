using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microservicios.Atracciones.Billing.DataManagement.Interfaces;
using Microservicios.Atracciones.Billing.DataManagement.Services;
using System.Reflection;

namespace Microservicios.Atracciones.Billing.DataManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddDataManagementServices(this IServiceCollection services)
    {
        // 1. Configurar Mapster
        var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        // Escanear el ensamblado actual para encontrar implementaciones de IRegister (ej: AttractionMapperConfig)
        typeAdapterConfig.Scan(Assembly.GetExecutingAssembly());
        
        services.AddSingleton(typeAdapterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        // 2. Registrar Data Services
        services.AddScoped<IPaymentDataService, PaymentDataService>();

        return services;
    }
}
