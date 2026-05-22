using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicios.Atracciones.Billing.DataAccess.Entities;

[Table("invoice")]
public class Invoice
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid BookingId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(20)]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string TaxId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    public string? Address { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Subtotal { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal TaxAmount { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal Total { get; set; }

    [Required]
    [MaxLength(3)]
    public string CurrencyCode { get; set; } = "USD";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}
