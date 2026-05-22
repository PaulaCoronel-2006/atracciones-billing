namespace Microservicios.Atracciones.Billing.Business.DTOs.Billing;

public class InvoiceSummaryResponse
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Emitida"; // Por ahora estático
}

public class InvoiceFullResponse : InvoiceSummaryResponse
{
    public string? Email { get; set; }
    public string? Address { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public List<InvoiceDetailResponse> Details { get; set; } = [];
}

public class InvoiceDetailResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TotalItem { get; set; }
}

public class CreateInvoiceRequest
{
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string CurrencyCode { get; set; } = "USD";
    public List<CreateInvoiceDetailRequest> Details { get; set; } = [];
}

public class CreateInvoiceDetailRequest
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Precio unitario (base o total? usualmente base)
    public decimal TaxRate { get; set; } // Ej: 15.00
}
