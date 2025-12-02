namespace Bolt.Domain.Exceptions
{
    /// <summary>
    /// Signals a business rule violation.
    /// </summary>
    public sealed class BusinessRuleViolationException : DomainException
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }
}
