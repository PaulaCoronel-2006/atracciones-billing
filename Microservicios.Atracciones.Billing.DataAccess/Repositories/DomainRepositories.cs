using Microservicios.Atracciones.Billing.DataAccess.Context;
using Microservicios.Atracciones.Billing.DataAccess.Entities;
using Microservicios.Atracciones.Billing.DataAccess.Repositories.Interfaces;

namespace Microservicios.Atracciones.Billing.DataAccess.Repositories;

public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository 
{ 
    public PaymentRepository(BillingDbContext context) : base(context) { } 
}

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{ 
    public InvoiceRepository(BillingDbContext context) : base(context) { } 
}

public class InvoiceDetailRepository : GenericRepository<InvoiceDetail>, IInvoiceDetailRepository
{ 
    public InvoiceDetailRepository(BillingDbContext context) : base(context) { } 
}
