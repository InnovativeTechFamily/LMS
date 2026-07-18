using Xunit;

namespace LMS.IntegrationTests;

/// <summary>
/// All integration tests share one API host + ephemeral MongoDB and run sequentially (xUnit does
/// not parallelise within a collection), which lets each test reset the database safely.
/// </summary>
[CollectionDefinition(Name)]
public class IntegrationCollection : ICollectionFixture<CustomWebApplicationFactory>
{
    public const string Name = "Integration";
}
