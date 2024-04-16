using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using ClassSchedule.Models;
using ClassSchedule.Controllers;
using System.Collections.Generic;

namespace ClassScheduleTests
{
    public class HomeControllerTests
    {
        [Fact]

        public void IndexActionMethod_ReturnsAViewResult()
        {
            // Arrange

            var mockDayRepository = new Mock<IRepository<Day>>();
            var mockClassRepository = new Mock<IRepository<Class>>();



            var expectedDays = new List<Day>
{
    new Day { DayId = 1, Name = "Monday" },
    new Day { DayId = 2, Name = "Tuesday" }
};
            mockDayRepository.Setup(repo => repo.List(It.IsAny<QueryOptions<Day>>()))
                             .Returns(expectedDays);

            var expectedClasses = new List<Class>
{
    new Class { ClassId = 1, Title = "Math", DayId = 1 },
    new Class { ClassId = 2, Title = "English", DayId = 2 }
};
            mockClassRepository.Setup(repo => repo.List(It.IsAny<QueryOptions<Class>>()))
                               .Returns(expectedClasses);

            var controller = new HomeController(mockDayRepository.Object, mockClassRepository.Object);

            // Act
            var result = controller.Index(0) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);


        }
    }
}