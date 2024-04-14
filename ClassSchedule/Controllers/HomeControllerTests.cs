using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClassSchedule.Models;
using ClassSchedule.Controllers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

public class HomeControllerTests
{
    private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
    private readonly Mock<ClassScheduleContext> mockContext;
    private readonly HomeController controller;

    public HomeControllerTests()
    {
        // Create a mock HttpContextAccessor
        mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession(); // Mock session if needed
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

        // Create a mock for the ClassScheduleContext (assuming this is your EF context)
        mockContext = new Mock<ClassScheduleContext>();

        // Initialize HomeController with mocked dependencies
        controller = new HomeController(mockContext.Object, mockHttpContextAccessor.Object);
    }

    [Fact]
    public void Index_ActionMethod_ReturnsAViewResult()
    {
        // Act
        var result = controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    // Helper class to mock session
    private class MockHttpSession : ISession
    {
        private Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();
        public string Id => "testsessionid";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear() => _sessionStorage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public void Remove(string key) => _sessionStorage.Remove(key);

        public void Set(string key, byte[] value) => _sessionStorage[key] = value;

        public bool TryGetValue(string key, out byte[] value)
        {
            if (_sessionStorage.ContainsKey(key))
            {
                value = (byte[])_sessionStorage[key];
                return true;
            }
            value = null;
            return false;
        }
    }
}
