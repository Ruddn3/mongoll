using System;
using System.Linq.Expressions;
using System.Threading;
using AutoFixture;
using CoreUnitTests.Infrastructure;
using CoreUnitTests.Infrastructure.Model;
using FluentAssertions;
using MongoDbGenericRepository.DataAccess.Read;
using Moq;
using Xunit;

namespace CoreUnitTests.KeyedReadOnlyMongoRepositoryTests;

public class GetByMaxTests : TestKeyedReadOnlyMongoRepositoryContext<int>
{
    private readonly Expression<Func<TestDocumentWithKey<int>, bool>> filter = document => document.SomeContent == "SomeContent";
    private readonly Expression<Func<TestDocumentWithKey<int>, object>> selector = document => document.SomeValue;

    [Fact]
    public void WithFilterAndSelector_GetsOne()
    {
        // Arrange
        var document = Fixture.Create<TestDocumentWithKey<int>>();

        SetupReader(document);

        // Act
        var result = Sut.GetByMax(filter, selector);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(document);
        Reader.Verify(
            x => x.GetByMax<TestDocumentWithKey<int>, int>(filter, selector, null, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void WithFilterAndSelectorAndCancellationToken_GetsOne()
    {
        // Arrange
        var document = Fixture.Create<TestDocumentWithKey<int>>();
        var token = new CancellationToken(true);

        SetupReader(document);

        // Act
        var result = Sut.GetByMax(filter, selector, token);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(document);
        Reader.Verify(
            x => x.GetByMax<TestDocumentWithKey<int>, int>(filter, selector, null, token),
            Times.Once);
    }

    [Fact]
    public void WithFilterAndSelectorAndPartitionKey_GetsOne()
    {
        // Arrange
        var document = Fixture.Create<TestDocumentWithKey<int>>();
        var partitionKey = Fixture.Create<string>();

        SetupReader(document);

        // Act
        var result = Sut.GetByMax(filter, selector, partitionKey);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(document);
        Reader.Verify(
            x => x.GetByMax<TestDocumentWithKey<int>, int>(filter, selector, partitionKey, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void WithFilterAndSelectorAndPartitionKeyAndCancellationToken_GetsOne()
    {
        // Arrange
        var document = Fixture.Create<TestDocumentWithKey<int>>();
        var partitionKey = Fixture.Create<string>();
        var token = new CancellationToken(true);

        SetupReader(document);

        // Act
        var result = Sut.GetByMax(filter, selector, partitionKey, token);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(document);
        Reader.Verify(
            x => x.GetByMax<TestDocumentWithKey<int>, int>(filter, selector, partitionKey, token),
            Times.Once);
    }

    private void SetupReader(TestDocumentWithKey<int> document)
    {
        Reader = new Mock<IMongoDbReader>();
        Reader
            .Setup(
                x => x.GetByMax<TestDocumentWithKey<int>, int>(
                    It.IsAny<Expression<Func<TestDocumentWithKey<int>, bool>>>(),
                    It.IsAny<Expression<Func<TestDocumentWithKey<int>, object>>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Returns(document);
    }
}
