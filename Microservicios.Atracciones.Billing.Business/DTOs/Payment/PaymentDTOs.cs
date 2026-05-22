namespace Microservicios.Atracciones.Billing.Business.DTOs.Payment;

public class PaymentResponse
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string? TransactionExternalId { get; set; }
    public short PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public short StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? RefundReason { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public Guid BookingId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Required]
    public short PaymentMethodId { get; set; }
    
    [System.ComponentModel.DataAnnotations.Required]
    public decimal Amount { get; set; }
    
    public string CurrencyCode { get; set; } = "USD";
    public string? TransactionExternalId { get; set; }
    public short? StatusId { get; set; }
}

public class UpdatePaymentStatusRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public short StatusId { get; set; }
    
    public string? TransactionExternalId { get; set; }
    public string? GatewayResponse { get; set; }
}

public class ProcessRefundRequest
{
    [System.ComponentModel.DataAnnotations.Required]
    public string RefundReason { get; set; } = string.Empty;
}
