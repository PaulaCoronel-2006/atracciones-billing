using Microsoft.EntityFrameworkCore;
using Microservicios.Atracciones.Billing.DataAccess.Entities;
using Microservicios.Atracciones.Billing.DataAccess.Repositories.Interfaces;
using Microservicios.Atracciones.Billing.DataManagement.Interfaces;

namespace Microservicios.Atracciones.Billing.DataManagement.Services;

public class PaymentDataService : IPaymentDataService
{
    private readonly IUnitOfWork _uow;

    public PaymentDataService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId)
    {
        // Se asume que el repositorio Payment existe en IUnitOfWork. Si no existe un repositorio específico,
        // podríamos usar el DbContext directamente, pero sigamos la convención.
        // Asumo que UnitOfWork expone Payments.
        return await _uow.Payments.Query()
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .Where(p => p.BookingId == bookingId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Payment?> GetPaymentByIdAsync(Guid id)
    {
        return await _uow.Payments.Query()
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Guid> AddPaymentAsync(Payment payment)
    {
        await _uow.Payments.AddAsync(payment);
        await _uow.CompleteAsync();
        return payment.Id;
    }

    public async Task<bool> UpdatePaymentAsync(Payment payment)
    {
        _uow.Payments.Update(payment);
        return await _uow.CompleteAsync() > 0;
    }
}
