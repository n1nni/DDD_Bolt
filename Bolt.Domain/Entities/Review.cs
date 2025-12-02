using Bolt.Domain.Abstractions;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Entities;

public class Review : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid DriverId { get; private set; }
    public Guid UserId { get; private set; }
    public Rating Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private Review() { }

    private Review(Guid id, Guid driverId, Guid userId, Rating rating, string comment)
    {
        Id = id;
        DriverId = driverId;
        UserId = userId;
        Rating = rating;
        Comment = comment?.Trim() ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Creates a new Review aggregate in a safe, validated manner.
    /// </summary>
    public static Result<Review> Create(Guid id, Guid driverId, Guid userId, Rating rating, string comment)
    {
        if (id == Guid.Empty)
            return Result<Review>.Failure("Review id cannot be empty.");

        if (driverId == Guid.Empty)
            return Result<Review>.Failure("Driver id cannot be empty.");

        if (userId == Guid.Empty)
            return Result<Review>.Failure("User id cannot be empty.");

        if (rating is null)
            return Result<Review>.Failure("Rating cannot be null.");

        // Optional domain rule: comment length
        if (!string.IsNullOrWhiteSpace(comment) && comment.Length > 500)
            return Result<Review>.Failure("Comment is too long (max 500 characters).");

        var review = new Review(id, driverId, userId, rating, comment);
        return Result<Review>.Success(review);
    }
}
