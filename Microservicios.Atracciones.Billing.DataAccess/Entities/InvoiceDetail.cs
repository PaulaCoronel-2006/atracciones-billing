using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microservicios.Atracciones.Billing.DataAccess.Entities;

[Table("invoice_detail")]
public class InvoiceDetail
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid InvoiceId { get; set; }

    [ForeignKey(nameof(InvoiceId))]
    public virtual Invoice Invoice { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(12,2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal TaxRate { get; set; } // Ej: 15.00

    [Column(TypeName = "decimal(12,2)")]
    public decimal TotalItem { get; set; }
}
