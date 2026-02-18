using System.Security.Claims;
using Alba;
using Alba.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using SunsetBooking.Domain.Base;
using SunsetBooking.Domain.HotelsRolodexFeature.Repositories;

namespace SunsetBooking.Tests.Base;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected IAlbaHost Host { get; private set; } = null!;
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";
    
    public IUserContext MockedUserContext;
    private AuthenticationStub _securityStub;
    
    public IntegrationTestBase()
    {
        MockedUserContext = Substitute.For<IUserContext>();
        var userId = "18263182736";
        var userFullname = "Dana Scully";

        MockedUserContext.UserId.Returns(userId);
        MockedUserContext.UserFullname.Returns(userFullname);

        _securityStub = new AuthenticationStub()
            .With(ClaimTypes.NameIdentifier, userId)
            .WithName(userFullname);
    }

    public async Task InitializeAsync()
    {
        var dbName = _dbName;
        Host = await AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove all existing HotelRolodexDbContext registrations (Npgsql)
                var descriptors = services
                    .Where(d => d.ServiceType == typeof(DbContextOptions<HotelRolodexDbContext>)
                             || d.ServiceType == typeof(HotelRolodexDbContext))
                    .ToList();
                foreach (var d in descriptors) services.Remove(d);

                // Add InMemory DbContext
                services.AddDbContext<HotelRolodexDbContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                // Replace UserContext with a test stub
                var userContextDescriptor = services
                    .SingleOrDefault(d => d.ServiceType == typeof(IUserContext));
                if (userContextDescriptor != null) services.Remove(userContextDescriptor);
                services.AddScoped<IUserContext>(_ => MockedUserContext);
            });
        }, _securityStub);
    }

    public async Task DisposeAsync()
    {
        await Host.DisposeAsync();
    }
}

internal class TestUserContext : IUserContext
{
    public string UserId => "test-user";
    public string UserFullname => "Dana Scully";
}
