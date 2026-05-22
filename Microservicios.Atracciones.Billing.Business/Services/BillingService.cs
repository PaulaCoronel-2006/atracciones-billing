using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microservicios.Atracciones.Billing.Business.DTOs.Billing;
using Microservicios.Atracciones.Billing.Business.Interfaces;
using Microservicios.Atracciones.Billing.DataAccess.Common;
using Microservicios.Atracciones.Billing.DataAccess.Repositories.Interfaces;
using Microservicios.Atracciones.Billing.DataAccess.Entities;

namespace Microservicios.Atracciones.Billing.Business.Services;

public class BillingService : IBillingService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _configuration;

    public BillingService(IUnitOfWork uow, IConfiguration configuration)
    {
        _uow = uow;
        _configuration = configuration;
    }

    public async Task<PagedResult<InvoiceSummaryResponse>> GetManagementInvoicesAsync(QueryFilters filters)
    {
        var query = _uow.Invoices.Query();

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            query = query.Where(i => i.InvoiceNumber.Contains(filters.SearchTerm) || 
                                     i.CustomerName.Contains(filters.SearchTerm) || 
                                     i.TaxId.Contains(filters.SearchTerm));
        }

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.CreatedAt)
                               .Skip(filters.Offset)
                               .Take(filters.PageSize)
                               .ToListAsync();

        var dtos = items.Select(i => new InvoiceSummaryResponse
        {
            Id = i.Id,
            BookingId = i.BookingId,
            InvoiceNumber = i.InvoiceNumber,
            CustomerName = i.CustomerName,
            TaxId = i.TaxId,
            Total = i.Total,
            Currency = i.CurrencyCode,
            CreatedAt = i.CreatedAt
        }).ToList();

        return new PagedResult<InvoiceSummaryResponse>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize
        };
    }

    public async Task<InvoiceFullResponse?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _uow.Invoices.Query()
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return null;

        return new InvoiceFullResponse
        {
            Id = invoice.Id,
            BookingId = invoice.BookingId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            TaxId = invoice.TaxId,
            Email = invoice.Email,
            Address = invoice.Address,
            Subtotal = invoice.Subtotal,
            TaxAmount = invoice.TaxAmount,
            Total = invoice.Total,
            Currency = invoice.CurrencyCode,
            CreatedAt = invoice.CreatedAt,
            Details = invoice.Details.Select(d => new InvoiceDetailResponse
            {
                Id = d.Id,
                Description = d.Description,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TaxRate = d.TaxRate,
                TotalItem = d.TotalItem
            }).ToList()
        };
    }

    public async Task<bool> CrearFacturaAsync(CreateInvoiceRequest request)
    {
        // Evitar duplicados
        var existing = await _uow.Invoices.Query().AnyAsync(i => i.BookingId == request.BookingId);
        if (existing) return true;

        decimal totalInvoice = 0;
        decimal totalTax = 0;
        decimal totalSubtotal = 0;

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = request.BookingId,
            UserId = request.UserId,
            InvoiceNumber = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{request.BookingId.ToString().Substring(0, 8)}",
            CustomerName = request.CustomerName,
            TaxId = request.TaxId,
            Email = request.Email,
            Address = request.Address,
            CurrencyCode = request.CurrencyCode,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var detailReq in request.Details)
        {
            decimal itemTotal = detailReq.UnitPrice * detailReq.Quantity;
            decimal itemSubtotal = itemTotal / (1 + (detailReq.TaxRate / 100));
            decimal itemTax = itemTotal - itemSubtotal;

            totalInvoice += itemTotal;
            totalSubtotal += itemSubtotal;
            totalTax += itemTax;

            var detail = new InvoiceDetail
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                Description = detailReq.Description,
                Quantity = detailReq.Quantity,
                UnitPrice = Math.Round(detailReq.UnitPrice / (1 + (detailReq.TaxRate / 100)), 2),
                TaxRate = detailReq.TaxRate,
                TotalItem = Math.Round(itemTotal, 2)
            };
            invoice.Details.Add(detail);
        }

        invoice.Total = Math.Round(totalInvoice, 2);
        invoice.Subtotal = Math.Round(totalSubtotal, 2);
        invoice.TaxAmount = Math.Round(totalTax, 2);

        await _uow.Invoices.AddAsync(invoice);
        await _uow.CompleteAsync();

        return true;
    }

    public async Task<IEnumerable<InvoiceSummaryResponse>> GetUserInvoicesAsync(Guid userId)
    {
        var items = await _uow.Invoices.Query()
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return items.Select(i => new InvoiceSummaryResponse
        {
            Id = i.Id,
            BookingId = i.BookingId,
            InvoiceNumber = i.InvoiceNumber,
            CustomerName = i.CustomerName,
            TaxId = i.TaxId,
            Total = i.Total,
            Currency = i.CurrencyCode,
            CreatedAt = i.CreatedAt
        }).ToList();
    }

    public async Task<bool> CancelarFacturaAsync(Guid bookingId)
    {
        var invoice = await _uow.Invoices.Query()
            .FirstOrDefaultAsync(i => i.BookingId == bookingId);

        if (invoice == null) return false;

        // Lógica de anulación (podría ser un cambio de estado si existiera la columna)
        return true;
    }
}
