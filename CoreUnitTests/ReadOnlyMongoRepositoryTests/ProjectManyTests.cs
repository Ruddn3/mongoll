using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using AutoFixture;
using CoreUnitTests.Infrastructure;
using CoreUnitTests.Infrastructure.Model;
using FluentAssertions;
using MongoDbGenericRepository.DataAccess.Read;
using Moq;
using Xunit;

namespace CoreUnitTests.ReadOnlyMongoRepositoryTests;

public class ProjectManyTests : TestReadOnlyMongoRepositoryContext
{
    private readonly Expression<Func<TestDocument, bool>> filter = document => document.SomeContent == "SomeContent";
    private readonly Expression<Func<TestDocument, TestProjection>> projection = document => new TestProjection {NestedData = document.Nested.SomeDate};

    [Fact]
    public void WithFilterAndProjection_Projects()
    {
        // Arrange
        var projections = Fixture.CreateMany<TestProjection>().ToList();

        SetupReader(projections);

        // Act
        var result = Sut.ProjectMany(filter, projection);

        // Assert
        result.Should().OnlyContain(x => projections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocument, TestProjection, Guid>(
                filter,
                projection,
                null,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void WithFilterAndProjectionAndCancellationToken_Projects()
    {
        // Arrange
        var projections = Fixture.CreateMany<TestProjection>().ToList();
        var token = new CancellationToken(true);

        SetupReader(projections);

        // Act
        var result = Sut.ProjectMany(filter, projection, token);

        // Assert
        result.Should().OnlyContain(x => projections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocument, TestProjection, Guid>(filter, projection, null, token),
            Times.Once);
    }

    [Fact]
    public void WithFilterAndProjectionAndPartitionKey_Projects()
    {
        // Arrange
        var projections = Fixture.CreateMany<TestProjection>().ToList();
        var partitionKey = Fixture.Create<string>();

        SetupReader(projections);

        // Act
        var result = Sut.ProjectMany(filter, projection, partitionKey);

        // Assert
        result.Should().OnlyContain(x => projections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocument, TestProjection, Guid>(filter, projection, partitionKey, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void WithFilterAndProjectionAndPartitionKeyAndCancellationToken_Projects()
    {
        // Arrange
        var projections = Fixture.CreateMany<TestProjection>().ToList();
        var partitionKey = Fixture.Create<string>();
        var token = new CancellationToken(true);

        SetupReader(projections);

        // Act
        var result = Sut.ProjectMany(filter, projection, partitionKey, token);

        // Assert
        result.Should().OnlyContain(x => projections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocument, TestProjection, Guid>(filter, projection, partitionKey, token),
            Times.Once);
    }

    private void SetupReader(List<TestProjection> projections)
    {
        Reader = new Mock<IMongoDbReader>();
        Reader
            .Setup(
                x => x.ProjectMany<TestDocument, TestProjection, Guid>(
                    It.IsAny<Expression<Func<TestDocument, bool>>>(),
                    It.IsAny<Expression<Func<TestDocument, TestProjection>>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Returns(projections);
    }

    #region Keyed

        private readonly Expression<Func<TestDocumentWithKey<int>, bool>> keyedFilter = document => document.SomeContent == "SomeContent";
    private readonly Expression<Func<TestDocumentWithKey<int>, TestProjection>> keyedProjection = document => new TestProjection {NestedData = document.Nested.SomeDate};

    [Fact]
    public void Keyed_WithFilterAndProjection_Projects()
    {
        // Arrange
        var keyedProjections = Fixture.CreateMany<TestProjection>().ToList();

        SetupKeyedReader(keyedProjections);

        // Act
        var result = Sut.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection);

        // Assert
        result.Should().OnlyContain(x => keyedProjections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(
                keyedFilter,
                keyedProjection,
                null,
                CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void Keyed_WithFilterAndProjectionAndCancellationToken_Projects()
    {
        // Arrange
        var keyedProjections = Fixture.CreateMany<TestProjection>().ToList();
        var token = new CancellationToken(true);

        SetupKeyedReader(keyedProjections);

        // Act
        var result = Sut.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection, token);

        // Assert
        result.Should().OnlyContain(x => keyedProjections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection, null, token),
            Times.Once);
    }

    [Fact]
    public void Keyed_WithFilterAndProjectionAndPartitionKey_Projects()
    {
        // Arrange
        var keyedProjections = Fixture.CreateMany<TestProjection>().ToList();
        var partitionKey = Fixture.Create<string>();

        SetupKeyedReader(keyedProjections);

        // Act
        var result = Sut.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection, partitionKey);

        // Assert
        result.Should().OnlyContain(x => keyedProjections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection, partitionKey, CancellationToken.None),
            Times.Once);
    }

    [Fact]
    public void Keyed_WithFilterAndProjectionAndPartitionKeyAndCancellationToken_Projects()
    {
        // Arrange
        var keyedProjections = Fixture.CreateMany<TestProjection>().ToList();
        var partitionKey = Fixture.Create<string>();
        var token = new CancellationToken(true);

        SetupKeyedReader(keyedProjections);

        // Act
        var result = Sut.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection, partitionKey, token);

        // Assert
        result.Should().OnlyContain(x => keyedProjections.Contains(x));
        Reader.Verify(
            x => x.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(keyedFilter, keyedProjection, partitionKey, token),
            Times.Once);
    }

    private void SetupKeyedReader(List<TestProjection> keyedProjections)
    {
        Reader = new Mock<IMongoDbReader>();
        Reader
            .Setup(
                x => x.ProjectMany<TestDocumentWithKey<int>, TestProjection, int>(
                    It.IsAny<Expression<Func<TestDocumentWithKey<int>, bool>>>(),
                    It.IsAny<Expression<Func<TestDocumentWithKey<int>, TestProjection>>>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
            .Returns(keyedProjections);
    }


    #endregion
}
