using System;

namespace Microservicios.Atracciones.Billing.Business.Exceptions;

/// <summary>
/// Excepción que se lanza cuando hay un conflicto con el estado actual del servidor (ej: recurso duplicado).
/// Se mapea a HTTP 409 Conflict.
/// </summary>
[System.Diagnostics.DebuggerNonUserCode]
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}
