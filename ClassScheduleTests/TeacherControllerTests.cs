using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ClassSchedule.Models;
using ClassSchedule.Controllers;
using Microsoft.EntityFrameworkCore;

namespace ClassScheduleTests
{
    public class TeacherControllerTests
    {
        [Fact]
        public void Index_ActionMethod_ReturnsAViewResult()
        {
            // Arrange
            var mockSet = new Mock<DbSet<Teacher>>();
            var mockContext = new Mock<ClassScheduleContext>();
            mockContext.Setup(m => m.Teachers).Returns(mockSet.Object);
            var controller = new TeacherController(mockContext.Object);

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }

}