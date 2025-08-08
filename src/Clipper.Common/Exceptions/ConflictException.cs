using System;

namespace Clipper.Common.Exceptions;

/// <summary>
/// Exceção para conflitos de dados no sistema
/// </summary>
/// <remarks>
/// Responsabilidade: representar erros de conflito como emails duplicados, recursos já existentes
/// </remarks>
public class ConflictException : Exception
{
    /// <summary>
    /// Inicializa uma nova instância da ConflictException
    /// </summary>
    public ConflictException() 
        : base("Conflito de dados detectado")
    {
    }

    /// <summary>
    /// Inicializa uma nova instância da ConflictException com mensagem específica
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    public ConflictException(string message) 
        : base(message)
    {
    }

    /// <summary>
    /// Inicializa uma nova instância da ConflictException com mensagem e exceção interna
    /// </summary>
    /// <param name="message">Mensagem de erro</param>
    /// <param name="innerException">Exceção interna</param>
    public ConflictException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
