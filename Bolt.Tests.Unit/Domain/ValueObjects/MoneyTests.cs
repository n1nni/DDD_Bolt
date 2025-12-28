using Bolt.Domain.ValueObjects;
using FluentAssertions;

namespace Bolt.Tests.Unit.Domain.ValueObjects;

public class MoneyTests
{
    [Theory]
    [InlineData(10.50, "GEL", "10.50 GEL")]
    [InlineData(0, "USD", "0.00 USD")]
    [InlineData(999.999, "EUR", "1000.00 EUR")] // Test rounding
    public void Constructor_WithValidParameters_CreatesMoney(decimal amount, string currency, string expectedString)
    {
        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(decimal.Round(amount, 2));
        money.Currency.Should().Be(currency.ToUpperInvariant());
        money.ToString().Should().Be(expectedString);
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-10));
    }

    [Fact]
    public void Constructor_WithNullCurrency_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Money(10, null!));
    }

    [Fact]
    public void Zero_ReturnsMoneyWithZeroAmount()
    {
        // Act
        var zeroMoney = Money.Zero("USD");

        // Assert
        zeroMoney.Amount.Should().Be(0);
        zeroMoney.Currency.Should().Be("USD");
    }

    [Theory]
    [InlineData(10.50, "GEL", 10.50, "GEL", true)]
    [InlineData(10.50, "GEL", 10.51, "GEL", false)]
    [InlineData(10.50, "GEL", 10.50, "USD", false)]
    public void Equals_ComparesAmountAndCurrency(decimal amount1, string currency1, decimal amount2, string currency2, bool expected)
    {
        // Arrange
        var money1 = new Money(amount1, currency1);
        var money2 = new Money(amount2, currency2);

        // Act & Assert
        money1.Equals(money2).Should().Be(expected);
    }
}
