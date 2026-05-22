namespace Microservicios.Atracciones.Billing.Business.Exceptions;

[System.Diagnostics.DebuggerNonUserCode]
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message) { }
}

