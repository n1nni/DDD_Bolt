namespace Bolt.Domain.Exceptions;

// Signals a business rule violation.
public sealed class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message) { }
}
