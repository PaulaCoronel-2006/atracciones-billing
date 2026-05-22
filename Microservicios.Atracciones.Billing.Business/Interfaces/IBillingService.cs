using Microservicios.Atracciones.Billing.Business.DTOs.Billing;
using Microservicios.Atracciones.Billing.DataAccess.Common;

namespace Microservicios.Atracciones.Billing.Business.Interfaces;

public interface IBillingService
{
    Task<PagedResult<InvoiceSummaryResponse>> GetManagementInvoicesAsync(QueryFilters filters);
    Task<InvoiceFullResponse?> GetInvoiceByIdAsync(Guid id);
    Task<IEnumerable<InvoiceSummaryResponse>> GetUserInvoicesAsync(Guid userId);
    Task<bool> CrearFacturaAsync(CreateInvoiceRequest request);
    Task<bool> CancelarFacturaAsync(Guid bookingId);
}
