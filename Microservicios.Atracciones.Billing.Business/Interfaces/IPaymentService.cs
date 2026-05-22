using Microservicios.Atracciones.Billing.Business.DTOs.Payment;

namespace Microservicios.Atracciones.Billing.Business.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentResponse>> GetPaymentsByBookingIdAsync(Guid bookingId);
    Task<PaymentResponse?> GetPaymentByIdAsync(Guid id);
    Task<Guid> CreatePaymentAsync(CreatePaymentRequest request);
    Task<bool> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request);
    Task<bool> ProcessRefundAsync(Guid id, ProcessRefundRequest request);
}
