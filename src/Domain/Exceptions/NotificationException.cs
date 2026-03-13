namespace NotificationService.Domain.Exceptions;

/// <summary>
/// Exceção de domínio para erros relacionados a notificações
/// </summary>
public class NotificationException : Exception
{
    public NotificationException(string message) : base(message)
    {
    }

    public NotificationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção lançada quando a validação de email falha
/// </summary>
public class InvalidEmailException : NotificationException
{
    public InvalidEmailException(string email) 
        : base($"Email inválido: {email}")
    {
    }
}

/// <summary>
/// Exceção lançada quando o envio de email falha
/// </summary>
public class EmailSendException : NotificationException
{
    public EmailSendException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
