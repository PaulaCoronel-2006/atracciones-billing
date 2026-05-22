using Microservicios.Atracciones.Billing.DataAccess.Entities;

namespace Microservicios.Atracciones.Billing.DataManagement.Interfaces;

public interface IPaymentDataService
{
    Task<IEnumerable<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId);
    Task<Payment?> GetPaymentByIdAsync(Guid id);
    Task<Guid> AddPaymentAsync(Payment payment);
    Task<bool> UpdatePaymentAsync(Payment payment);
}
