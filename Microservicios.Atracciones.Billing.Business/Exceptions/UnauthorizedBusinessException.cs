namespace Microservicios.Atracciones.Billing.Business.Exceptions;

[System.Diagnostics.DebuggerNonUserCode]
public class UnauthorizedBusinessException : Exception
{
    public UnauthorizedBusinessException(string message) : base(message) { }
}
