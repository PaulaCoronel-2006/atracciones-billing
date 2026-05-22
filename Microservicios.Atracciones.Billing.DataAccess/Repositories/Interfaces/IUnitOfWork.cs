namespace Microservicios.Atracciones.Billing.DataAccess.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPaymentRepository Payments { get; }
    IInvoiceRepository Invoices { get; }
    IInvoiceDetailRepository InvoiceDetails { get; }

    Task<int> CompleteAsync();
}
