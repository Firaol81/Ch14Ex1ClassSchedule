using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using ClassSchedule.Models;
using ClassSchedule.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

public class HomeControllerTests
{
    private readonly HomeController controller;
    private readonly Mock<IHttpContextAccessor> mockHttpContextAccessor;
    private readonly Mock<ClassScheduleContext> mockContext;
    private readonly Mock<ClassScheduleUnitOfWork> mockUnitOfWork;

    public HomeControllerTests()
    {
        // Mock the HttpContextAccessor
        mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession(); // MockHttpSession should be defined to mimic HttpSession functionality
        mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

        // Mock the ClassScheduleContext
        mockContext = new Mock<ClassScheduleContext>();

        // Mock the ClassScheduleUnitOfWork
        mockUnitOfWork = new Mock<ClassScheduleUnitOfWork>(mockContext.Object);

        // Setup mock data or behavior if needed
        mockUnitOfWork.Setup(m => m.Days.List(It.IsAny<QueryOptions<Day>>())).Returns(new List<Day>());
        mockUnitOfWork.Setup(m => m.Classes.List(It.IsAny<QueryOptions<Class>>())).Returns(new List<Class>());

        // Initialize the HomeController with mocked dependencies
        controller = new HomeController(mockContext.Object, mockHttpContextAccessor.Object);
    }

    [Fact]
    public void Index_ActionMethod_ReturnsAViewResult()
    {
        // Act
        var result = controller.Index(0);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public void Index_ActionMethod_SetsSessionCorrectly_WhenIdGreaterThanZero()
    {
        // Arrange
        int testId = 1;
        var sessionMock = Mock.Get((ISession)mockHttpContextAccessor.Object.HttpContext.Session);
        sessionMock.Setup(s => s.SetInt32("dayid", testId)).Verifiable();

        // Act
        var result = controller.Index(testId);

        // Assert
        sessionMock.Verify(s => s.SetInt32("dayid", testId), Times.Once);
    }

    [Fact]
    public void Cancel_Action_RedirectsToIndex()
    {
        // Arrange
        int testId = 1;
        var sessionMock = Mock.Get((ISession)mockHttpContextAccessor.Object.HttpContext.Session);
        sessionMock.Setup(s => s.GetInt32("dayid")).Returns(testId);

        // Act
        var result = controller.Cancel() as RedirectToActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Index", result.ActionName);
        Assert.Equal(testId, result.RouteValues["id"]);
    }
}

