using Microsoft.EntityFrameworkCore;
using Microservicios.Atracciones.Billing.Business.DTOs.Payment;
using Microservicios.Atracciones.Billing.Business.Interfaces;
using Microservicios.Atracciones.Billing.DataAccess.Repositories.Interfaces;
using Microservicios.Atracciones.Billing.DataAccess.Entities;

namespace Microservicios.Atracciones.Billing.Business.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _uow;

    public PaymentService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<PaymentResponse>> GetPaymentsByBookingIdAsync(Guid bookingId)
    {
        var payments = await _uow.Payments.Query()
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .Where(p => p.BookingId == bookingId)
            .ToListAsync();

        return payments.Select(MapToResponse);
    }

    public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _uow.Payments.Query()
            .Include(p => p.PaymentMethod)
            .Include(p => p.Status)
            .FirstOrDefaultAsync(p => p.Id == id);

        return payment == null ? null : MapToResponse(payment);
    }

    public async Task<Guid> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = request.BookingId,
            PaymentMethodId = request.PaymentMethodId,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            TransactionExternalId = request.TransactionExternalId,
            StatusId = request.StatusId ?? 1,
            PaidAt = request.StatusId == 2 ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _uow.Payments.AddAsync(payment);
        await _uow.CompleteAsync();
        return payment.Id;
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request)
    {
        var payment = await _uow.Payments.GetByIdAsync(id);
        if (payment == null) return false;

        payment.StatusId = request.StatusId;
        
        if (!string.IsNullOrEmpty(request.TransactionExternalId))
            payment.TransactionExternalId = request.TransactionExternalId;
            
        if (!string.IsNullOrEmpty(request.GatewayResponse))
            payment.GatewayResponse = request.GatewayResponse;

        if (request.StatusId == 2) // Succeeded
        {
            payment.PaidAt = DateTime.UtcNow;
        }

        _uow.Payments.Update(payment);
        await _uow.CompleteAsync();
        return true;
    }

    public async Task<bool> ProcessRefundAsync(Guid id, ProcessRefundRequest request)
    {
        var payment = await _uow.Payments.GetByIdAsync(id);
        if (payment == null) return false;

        if (payment.StatusId != 2) // Only Paid can be refunded
            throw new InvalidOperationException("Solo los pagos completados pueden ser reembolsados.");

        payment.StatusId = 4; // Refunded
        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundReason = request.RefundReason;
        payment.UpdatedAt = DateTime.UtcNow;

        _uow.Payments.Update(payment);
        await _uow.CompleteAsync();
        return true;
    }

    private static PaymentResponse MapToResponse(Payment p)
    {
        return new PaymentResponse
        {
            Id = p.Id,
            BookingId = p.BookingId,
            TransactionExternalId = p.TransactionExternalId,
            PaymentMethodId = p.PaymentMethodId,
            PaymentMethodName = p.PaymentMethod?.Name ?? string.Empty,
            StatusId = p.StatusId,
            StatusName = p.Status?.Name ?? string.Empty,
            Amount = p.Amount,
            CurrencyCode = p.CurrencyCode,
            PaidAt = p.PaidAt,
            RefundedAt = p.RefundedAt,
            RefundReason = p.RefundReason,
            CreatedAt = p.CreatedAt
        };
    }
}
