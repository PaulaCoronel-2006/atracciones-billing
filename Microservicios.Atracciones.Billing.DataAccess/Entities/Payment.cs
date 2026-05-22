namespace Microservicios.Atracciones.Billing.DataAccess.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public string? TransactionExternalId { get; set; }  // ID de Stripe / PayPal
    public short PaymentMethodId { get; set; }
    public short StatusId { get; set; } = 1;            // 1 = Pending
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public string? GatewayResponse { get; set; }        // JSON crudo del gateway
    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual PaymentMethodType PaymentMethod { get; set; } = null!;
    public virtual PaymentStatusType Status { get; set; } = null!;
}
