using MassTransit;
using Microservicios.Atracciones.Billing.Business.DTOs.Billing;
using Microservicios.Atracciones.Billing.Business.Interfaces;
using Microservicios.Atracciones.Common.Events;

namespace Microservicios.Atracciones.Billing.Business.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreatedEvent>
{
    private readonly IBillingService _billingService;

    public BookingCreatedConsumer(IBillingService billingService)
    {
        _billingService = billingService;
    }

    public async Task Consume(ConsumeContext<BookingCreatedEvent> context)
    {
        var @event = context.Message;

        var request = new CreateInvoiceRequest
        {
            BookingId = @event.BookingId,
            UserId = @event.UserId,
            CustomerName = string.IsNullOrEmpty(@event.CustomerName) ? "Consumidor Final" : @event.CustomerName,
            TaxId = string.IsNullOrEmpty(@event.TaxId) ? "9999999999999" : @event.TaxId,
            Email = @event.Email,
            Address = @event.Address,
            CurrencyCode = @event.CurrencyCode,
            Details = new List<CreateInvoiceDetailRequest>
            {
                new()
                {
                    Description = $"Reserva de Atracción: {@event.AttractionName} (PNR: {@event.PnrCode})",
                    Quantity = 1,
                    UnitPrice = @event.TotalAmount,
                    TaxRate = 15.00m
                }
            }
        };

        await _billingService.CrearFacturaAsync(request);
    }
}
