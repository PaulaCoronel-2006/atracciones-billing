namespace Microservicios.Atracciones.Billing.DataAccess.Entities;

public class PaymentMethodType
{
    public short Id { get; set; }
    public string Name { get; set; } = string.Empty;   // Card | Transfer | Cash | PayPal | Crypto
    public virtual ICollection<Payment> Payments { get; set; } = [];
}
