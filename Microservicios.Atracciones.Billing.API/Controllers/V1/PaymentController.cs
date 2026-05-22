using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microservicios.Atracciones.Billing.Business.DTOs.Payment;
using Microservicios.Atracciones.Billing.Business.Interfaces;

namespace Microservicios.Atracciones.Billing.API.Controllers.V1;

[ApiController]
[Route("api/v1/payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("booking/{bookingId:guid}")]
    [Authorize] // Dependiendo de la lógica, podría ser cualquier usuario autenticado o solo el dueño
    public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetPaymentsByBooking(Guid bookingId)
    {
        var result = await _paymentService.GetPaymentsByBookingIdAsync(bookingId);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<PaymentResponse>> GetById(Guid id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Guid>> Create([FromBody] CreatePaymentRequest request)
    {
        var id = await _paymentService.CreatePaymentAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:guid}/status")]
    [Authorize] // Permitimos que el Frontend (Cliente) actualice el estado tras el cobro exitoso
    public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusRequest request)
    {
        var success = await _paymentService.UpdatePaymentStatusAsync(id, request);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("{id:guid}/refund")]
    [Authorize(Roles = "Admin,Partner")] // Solo administradores
    public async Task<ActionResult> Refund(Guid id, [FromBody] ProcessRefundRequest request)
    {
        try
        {
            var success = await _paymentService.ProcessRefundAsync(id, request);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
