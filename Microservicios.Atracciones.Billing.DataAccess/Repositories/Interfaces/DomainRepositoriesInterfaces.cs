using Microservicios.Atracciones.Billing.DataAccess.Entities;

namespace Microservicios.Atracciones.Billing.DataAccess.Repositories.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment> { }
public interface IInvoiceRepository : IGenericRepository<Invoice> { }
public interface IInvoiceDetailRepository : IGenericRepository<InvoiceDetail> { }
