using Bolt.Domain.ValueObjects;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.ValueObjects;

public class RatingTests
{
    [Theory]
    [InlineData(4.5)]
    [InlineData(0.0)]
    [InlineData(5.0)]
    public void Constructor_WithValidRating_CreatesRating(double value)
    {
        // Act
        var rating = new Rating(value);

        // Assert
        rating.Value.Should().Be(value);
        rating.TotalReviews.Should().Be(1);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(5.1)]
    public void Constructor_WithInvalidRating_ThrowsException(double value)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Rating(value));
    }

    [Fact]
    public void Constructor_WithNegativeTotalReviews_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Rating(4.0, -1));
    }

    [Fact]
    public void UpdateWith_AddsNewRatingAndCalculatesAverage()
    {
        // Arrange
        var initialRating = new Rating(4.0, 2); // Average 4.0 from 2 reviews

        // Act
        var updatedRating = initialRating.UpdateWith(5.0);

        // Assert - Value gets rounded to 1 decimal place in constructor
        updatedRating.Value.Should().BeApproximately(4.3, 0.05); // 4.3 ± 0.05
        updatedRating.TotalReviews.Should().Be(3);
    }

    [Fact]
    public void UpdateWith_InvalidRating_ThrowsException()
    {
        // Arrange
        var rating = new Rating(4.0);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => rating.UpdateWith(5.1));
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        // Arrange
        var rating = new Rating(4.5, 10);

        // Act
        var result = rating.ToString();

        // Assert
        result.Should().Be("4.5 (10 reviews)");
    }
}
