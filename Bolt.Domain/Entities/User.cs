// Bolt.Domain.Entities/User.cs
using Bolt.Domain.Abstractions;
using Bolt.Domain.Enums;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;
using Bolt.Domain.Exceptions; // for [Owned]

namespace Bolt.Domain.Entities
{
    /// <summary>
    /// Single User aggregate. Role determines whether user is a driver or passenger.
    /// Driver-specific data (vehicle, availability) is represented in DriverProfile.
    /// </summary>
    public class User : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public UserRole Role { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        protected User() { } // EF Core

        protected User(Guid id, string fullName, string email, string phoneNumber, UserRole role)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            PhoneNumber = phoneNumber;
            Role = role;
            CreatedAt = DateTime.UtcNow;
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

            Console.WriteLine($"[LOG] User created: {user.Id} - {user.FullName} ({user.Role})");
            return Result<User>.Success(user);
        }

        public void UpdateProfile(string fullName, string email, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

            FullName = fullName.Trim();
            Email = email.Trim().ToLowerInvariant();
            PhoneNumber = phoneNumber.Trim();

            Console.WriteLine($"[LOG] User profile updated: {Id}");
        }

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            Console.WriteLine($"[LOG] User login recorded: {Id} at {LastLoginAt}");
        }
    }

}
