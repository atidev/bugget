using System.Security.Claims;
using System.Text.Encodings.Web;
using Bugget.Authentication;
using Bugget.DA.Interfaces;
using Bugget.Entities.BO;
using Bugget.Entities.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Bugget.Tests.Authentication;

public class UserAuthHandlerTests
{
    private static UserAuthHandler CreateHandler(
        DefaultHttpContext context,
        Mock<IEmployeesClient> clientMock,
        AuthHeadersOptions headersOptions)
    {
        // Mock AuthenticationSchemeOptions
        var authSchemeOptions = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        authSchemeOptions
            .Setup(o => o.Get(It.IsAny<string?>()))
            .Returns(new AuthenticationSchemeOptions());

        // Mock AuthHeadersOptions
        var headersOptionsMonitor = new Mock<IOptionsMonitor<AuthHeadersOptions>>();
        headersOptionsMonitor
            .Setup(o => o.CurrentValue)
            .Returns(headersOptions);

        var handler = new UserAuthHandler(
            authSchemeOptions.Object,
            LoggerFactory.Create(_ => { }),
            UrlEncoder.Default,
            clientMock.Object,
            headersOptionsMonitor.Object);

        handler.InitializeAsync(
            new AuthenticationScheme("headers", null, typeof(UserAuthHandler)),
            context).Wait();
        return handler;
    }

    [Fact]
    public async Task Succeeds_WhenAllHeadersAreValid()
    {
        // Arrange
        var headersOptions = new AuthHeadersOptions
        {
            UserIdHeaderName = "X-User-Id",
            TeamIdHeaderName = "X-Team-Id",
            OrganizationIdHeaderName = "X-Org-Id"
        };
        var context = new DefaultHttpContext();
        context.Request.Headers[headersOptions.UserIdHeaderName] = "user-123";
        context.Request.Headers[headersOptions.TeamIdHeaderName] = "team-456";
        context.Request.Headers[headersOptions.OrganizationIdHeaderName] = "org-789";
        context.Request.Headers["X-Signal-R-Conntection-Id"] = "conn-abc";

        var employeesClient = new Mock<IEmployeesClient>();
        var handler = CreateHandler(context, employeesClient, headersOptions);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
        var claims = result.Principal!.Claims.ToDictionary(c => c.Type, c => c.Value);
        Assert.Equal("user-123", claims[ClaimTypes.NameIdentifier]);
        Assert.Equal("team-456", claims["team_id"]);
        Assert.Equal("org-789", claims["organization_id"]);
        Assert.Equal("conn-abc", claims["signalr_connection_id"]);
    }

    [Fact]
    public async Task Fails_WhenUserHeaderConfiguredButMissing()
    {
        // Arrange: only user header configured
        var headersOptions = new AuthHeadersOptions
        {
            UserIdHeaderName = "X-User-Id"
        };
        var context = new DefaultHttpContext(); // no headers

        var employeesClient = new Mock<IEmployeesClient>();
        var handler = CreateHandler(context, employeesClient, headersOptions);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("User ID not found", result.Failure?.Message);
    }

    [Fact]
    public async Task Succeeds_WhenUserHeaderMissingButNotConfigured()
    {
        // Arrange: no headers configured
        var headersOptions = new AuthHeadersOptions();
        var context = new DefaultHttpContext();

        var clientMock = new Mock<IEmployeesClient>();
        clientMock.Setup(c => c.GetEmployeeAsync("default-user"))
            .ReturnsAsync(new Employee
            {
                TeamId = "fallback-team",
                Id = null
            });

        var handler = CreateHandler(context, clientMock, headersOptions);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
        Assert.Equal("default-user", result.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        Assert.Equal("fallback-team", result.Principal?.FindFirst("team_id")?.Value);
    }

    [Fact]
    public async Task Fails_WhenTeamHeaderConfiguredButEmpty()
    {
        // Arrange
        var headersOptions = new AuthHeadersOptions
        {
            UserIdHeaderName = "X-User-Id",
            TeamIdHeaderName = "X-Team-Id"
        };
        var context = new DefaultHttpContext();
        context.Request.Headers[headersOptions.UserIdHeaderName] = "user-123";
        context.Request.Headers[headersOptions.TeamIdHeaderName] = string.Empty;

        var clientMock = new Mock<IEmployeesClient>();
        var handler = CreateHandler(context, clientMock, headersOptions);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Team ID not found", result.Failure?.Message);
    }

    [Fact]
    public async Task Fails_WhenOrgHeaderConfiguredButEmpty()
    {
        // Arrange
        var headersOptions = new AuthHeadersOptions
        {
            UserIdHeaderName = "X-User-Id",
            OrganizationIdHeaderName = "X-Org-Id"
        };
        var context = new DefaultHttpContext();
        context.Request.Headers[headersOptions.UserIdHeaderName] = "user-123";
        context.Request.Headers[headersOptions.OrganizationIdHeaderName] = string.Empty;

        var clientMock = new Mock<IEmployeesClient>();
        clientMock.Setup(c => c.GetEmployeeAsync("user-123"))
            .ReturnsAsync(new Employee
            {
                TeamId = "fallback-team",
                Id = null
            });

        var handler = CreateHandler(context, clientMock, headersOptions);

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Organization ID not found", result.Failure?.Message);
    }
}