namespace Microservicios.Atracciones.Billing.DataAccess.Entities;

public class PaymentStatusType
{
    public short Id { get; set; }
    public string Name { get; set; } = string.Empty;   // Pending | Succeeded | Failed | Refunded | Disputed
    public virtual ICollection<Payment> Payments { get; set; } = [];
}
