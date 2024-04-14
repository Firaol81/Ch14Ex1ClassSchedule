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
    private readonly HomeControllerTests ontroller;

    public HomeControllerTests()
    {
        // Create mock for IHttpContextAccessor
        mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession(); // Ensure the session is set up correctly.
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

        // Create mock for ClassScheduleContext (if it's your Entity Framework context)
        mockContext = new Mock<ClassScheduleContext>();

        // Create the HomeController with mocked dependencies
        controller = new HomeController(mockContext.Object, mockHttpContextAccessor.Object);
    }

    [Fact]
    public void Index_ActionMethod_ReturnsAViewResult()
    {
        // Act
        var result = controller.Index() as ViewResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ViewResult>(result);
    }

    // MockHttpSession class defined within HomeControllerTests
    private class MockHttpSession : ISession
    {
        private Dictionary<string, object> _sessionStorage = new Dictionary<string, object>();
        public string Id => "test_session_id";
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionStorage.Keys;

        public void Clear()
        {
            _sessionStorage.Clear();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _sessionStorage.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _sessionStorage[key] = value;
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (_sessionStorage.TryGetValue(key, out object objValue))
            {
                value = (byte[])objValue;
                return true;
            }
            value = null;
            return false;
        }
    }
}
