namespace ShoeEcommerce.Application.Common.Exceptions;
public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}

public class BadRequestException : ApplicationException
{
    public BadRequestException(string message) : base(message)
    {
    }
}

public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
public class NotFoundException : ApplicationException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with ID '{key}' was not found")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message)
    {
    }
}

public class ValidationException : ApplicationException
{

    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
}

public class AccountLockedException : ApplicationException
{
    public DateTime? LockoutEnd { get; }

    public AccountLockedException(string message, DateTime? lockoutEnd = null)
        : base(message)
    {
        LockoutEnd = lockoutEnd;
    }
}

public class AccountBlockedException : ApplicationException
{

    public string? Reason { get; }
    public DateTime? ExpiresAt { get; }

    public AccountBlockedException(string message, string? reason = null, DateTime? expiresAt = null)
        : base(message)
    {
        Reason = reason;
        ExpiresAt = expiresAt;
    }
}

public class EmailNotConfirmedException : ApplicationException
{
    public EmailNotConfirmedException()
        : base("Email address has not been confirmed. Please check your email for confirmation link.")
    {
    }

    public EmailNotConfirmedException(string message) : base(message)
    {
    }
}
public class InvalidRefreshTokenException : ApplicationException
{
    public InvalidRefreshTokenException(string message) : base(message)
    {
    }

    public InvalidRefreshTokenException()
        : base("Invalid or expired refresh token. Please login again.")
    {
    }
}