using Microsoft.Extensions.DependencyInjection;
using Microservicios.Atracciones.Billing.Business.Interfaces;
using Microservicios.Atracciones.Billing.Business.Services;

namespace Microservicios.Atracciones.Billing.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IBillingService, BillingService>();

        return services;
    }
}
