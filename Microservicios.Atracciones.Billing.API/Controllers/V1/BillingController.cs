using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microservicios.Atracciones.Billing.Business.DTOs.Billing;
using Microservicios.Atracciones.Billing.Business.Interfaces;
using Microservicios.Atracciones.Billing.DataAccess.Common;

namespace Microservicios.Atracciones.Billing.API.Controllers.V1;

[ApiController]
[Route("api/v1/billing")]
[Authorize(Roles = "Admin,Partner,Client")]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;

    public BillingController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    [HttpGet("management")]
    public async Task<ActionResult<PagedResult<InvoiceSummaryResponse>>> GetManagementInvoices([FromQuery] QueryFilters filters)
    {
        var result = await _billingService.GetManagementInvoicesAsync(filters);
        return Ok(result);
    }
 
    [HttpGet("my-invoices")]
    public async Task<ActionResult<IEnumerable<InvoiceSummaryResponse>>> GetMyInvoices()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null) return Unauthorized();
 
        var userId = Guid.Parse(userIdClaim.Value);
        var result = await _billingService.GetUserInvoicesAsync(userId);
        return Ok(result);
    }

    [HttpGet("management/{id:guid}")]
    public async Task<ActionResult<InvoiceFullResponse>> GetInvoiceDetail(Guid id)
    {
        var result = await _billingService.GetInvoiceByIdAsync(id);
        
        if (result == null)
            return NotFound(new { message = "La factura no existe." });

        return Ok(result);
    }

    [HttpPost("invoice")]
    public async Task<ActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var result = await _billingService.CrearFacturaAsync(request);
        
        if (!result)
            return BadRequest(new { message = "No se pudo generar la factura." });

        return Ok(new { message = "Factura generada y registrada con éxito." });
    }

    [HttpPost("management/{id:guid}/void")]
    public async Task<ActionResult> VoidInvoice(Guid id)
    {
        var invoice = await _billingService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();

        var result = await _billingService.CancelarFacturaAsync(invoice.BookingId);
        
        if (!result)
            return BadRequest(new { message = "No se pudo anular la factura." });

        return Ok(new { message = "Factura marcada para anulación." });
    }
}
