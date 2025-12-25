using Bolt.Application.Abstractions;
using Bolt.Application.Common;
using Bolt.Domain.Entities;
using Bolt.Domain.Repositories;
using Bolt.Domain.ValueObjects;
using MediatR;

namespace Bolt.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<Guid>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRideOrderRepository _rideRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewCommandHandler(
        IReviewRepository reviewRepository,
        IUserRepository userRepository,
        IRideOrderRepository rideRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _rideRepository = rideRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
    CreateReviewCommand request,
    CancellationToken cancellationToken)
    {
        // Verify ride exists and is completed
        var ride = await _rideRepository.GetByIdAsync(request.RideId, cancellationToken);
        if (ride == null)
        {
            return Result.Failure<Guid>("Ride not found.");
        }

        if (ride.Status != Domain.Enums.RideStatus.Completed)
        {
            return Result.Failure<Guid>("Can only review completed rides.");
        }

        // Get driver and passenger
        var driver = await _userRepository.GetDriverByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
        {
            return Result.Failure<Guid>("Driver not found.");
        }

        var passenger = await _userRepository.GetPassengerByIdAsync(
            request.PassengerId,
            cancellationToken);

        if (passenger == null)
        {
            return Result.Failure<Guid>("Passenger not found.");
        }

        // **CRITICAL FIX**: Check if this passenger has already reviewed this driver
        var existingReview = await _reviewRepository.GetByDriverAndPassengerAsync(
            driver.Id,
            passenger.Id,
            cancellationToken);

        if (existingReview != null)
        {
            return Result.Failure<Guid>("You have already reviewed this driver.");
        }

        // Alternative: Check if review exists for this specific ride
        var existingReviewForRide = await _reviewRepository.GetByRideIdAsync(
            request.RideId,
            cancellationToken);

        if (existingReviewForRide != null)
        {
            return Result.Failure<Guid>("Review already exists for this ride.");
        }

        var rating = new Rating(request.RatingValue);
        var reviewResult = Review.Create(
            Guid.NewGuid(),
            request.RideId,
            driver,
            passenger,
            rating,
            request.Comment);

        if (!reviewResult.IsSuccess)
        {
            return Result.Failure<Guid>(reviewResult.Error!);
        }

        // Update driver's rating
        if (driver.Rating == null)
        {
            driver.UpdateRating(rating);
        }
        else
        {
            var updatedRating = driver.Rating.UpdateWith(request.RatingValue);
            driver.UpdateRating(updatedRating);
        }

        await _reviewRepository.AddAsync(reviewResult.Value!, cancellationToken);
        _userRepository.Update(driver);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(reviewResult.Value!.Id);
    }
}
