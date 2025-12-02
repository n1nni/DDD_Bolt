using Bolt.Domain.Abstractions;
using Bolt.Domain.Enums;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Entities;

public class User : IAggregateRoot
{
    public Guid Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Rating? Rating { get; private set; }

    private User() { } // ORM

    private User(Guid id, string fullName, string email, UserRole role)
    {
        Id = id;
        FullName = fullName;
        Email = email;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }

    // FACTORY (Result<T>)
    public static Result<User> Create(Guid id, string fullName, string email, UserRole role = UserRole.User)
    {
        if (id == Guid.Empty)
            return Result<User>.Failure("User ID cannot be empty.");

        if (string.IsNullOrWhiteSpace(fullName))
            return Result<User>.Failure("Full name is required.");

        if (string.IsNullOrWhiteSpace(email))
            return Result<User>.Failure("Email is required.");

        var user = new User(id, fullName.Trim(), email.Trim().ToLowerInvariant(), role);

        return Result<User>.Success(user);
    }

    // BEHAVIOR
    public void UpdateProfile(string fullName, string email)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
    }

    public void UpdateRating(Rating rating)
    {
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
    }
}