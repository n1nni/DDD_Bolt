using Bolt.Domain.Abstractions;
using Bolt.Domain.Shared;
using Bolt.Domain.ValueObjects;

namespace Bolt.Domain.Entities;

public class Review : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid RideId { get; private set; }
    public Guid DriverId { get; private set; }
    public Guid PassengerId { get; private set; }
    public Rating Rating { get; private set; }
    public string Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    private Review() { }

    private Review(
        Guid id,
        Guid rideId,
        Guid driverId,
        Guid passengerId,
        Rating rating,
        string comment)
    {
        Id = id;
        RideId = rideId;
        DriverId = driverId;
        PassengerId = passengerId;
        Rating = rating;
        Comment = comment?.Trim() ?? string.Empty;
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    // Simple soft delete methods
    public void MarkAsDeleted()
    {
        IsDeleted = true;
        Console.WriteLine($"[LOG] Review marked as deleted: {Id}");
    }

    public void Restore()
    {
        IsDeleted = false;
        Console.WriteLine($"[LOG] Review restored: {Id}");
    }

    public static Result<Review> Create(
        Guid id,
        Guid rideId,
        Driver driver,
        Passenger passenger,
        Rating rating,
        string comment)
    {
        if (id == Guid.Empty)
            return Result<Review>.Failure("Review ID cannot be empty.");

        if (rideId == Guid.Empty)
            return Result<Review>.Failure("Ride ID cannot be empty.");

        if (driver == null)
            return Result<Review>.Failure("Driver cannot be null.");

        if (passenger == null)
            return Result<Review>.Failure("Passenger cannot be null.");

        if (rating == null)
            return Result<Review>.Failure("Rating cannot be null.");

        if (!string.IsNullOrWhiteSpace(comment) && comment.Length > 500)
            return Result<Review>.Failure("Comment cannot exceed 500 characters.");

        var review = new Review(id, rideId, driver.Id, passenger.Id, rating, comment);
        Console.WriteLine($"[LOG] Review created: {id} for Driver {driver.Id} by Passenger {passenger.Id}");

        return Result<Review>.Success(review);
    }
}
