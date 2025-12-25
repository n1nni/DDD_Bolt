using Bolt.Domain.Abstractions;
using Bolt.Domain.Enums;
using Bolt.Domain.Events;
using Bolt.Domain.Shared;


namespace Bolt.Domain.Entities;

public class User : AggregateRoot
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public bool IsDeleted { get; private set; }

    protected User() { }

    protected User(Guid id, string fullName, string email, string phoneNumber, UserRole role)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static Result<User> Create(
        Guid id,
        string fullName,
        string email,
        string phoneNumber,
        UserRole role)
    {
        if (id == Guid.Empty)
            return Result<User>.Failure("User ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(fullName))
            return Result<User>.Failure("Full name is required.");

        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure("Email is required.");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return Result<User>.Failure("Phone number is required.");

        User user = role switch
        {
            UserRole.Driver => new Driver(id, fullName.Trim(), email.Trim().ToLowerInvariant(),
                                         phoneNumber.Trim()),
            UserRole.Passenger => new Passenger(id, fullName.Trim(), email.Trim().ToLowerInvariant(),
                                               phoneNumber.Trim()),
            _ => throw new ArgumentOutOfRangeException(nameof(role))
        };

        // Add domain event
        user.AddDomainEvent(new UserCreatedEvent(id, role.ToString()));

        Console.WriteLine($"[LOG] User created: {user.Id} - {user.FullName} ({user.Role})");
        return Result<User>.Success(user);
    }
}
