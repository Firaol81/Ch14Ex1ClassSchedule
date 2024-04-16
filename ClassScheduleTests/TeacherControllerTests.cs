using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using ClassSchedule.Models;
using ClassSchedule.Controllers;
using System.Collections.Generic;




namespace ClassScheduleTests
{


    public class TeacherControllerTests
    {
        [Fact]
        public void IndexActionMethod_ReturnsAViewResult()
        {

            //Arrange
            var mockRepository = new Mock<IRepository<Teacher>>();

            //
            var controller = new TeacherController(mockRepository.Object);



            //Act
            var result = controller.Index();


            //Assert
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public void IndexActionMethod_ModelIsAListOfTeacherObjects()
        {
            // Arrange
            var mockRepository = new Mock<IRepository<Teacher>>();

            // Setup mock repository 
            var expectedTeachers = new List<Teacher>
            {
                new Teacher { TeacherId = 1, FirstName = "John ", LastName = "Doe" },
                new Teacher { TeacherId = 2, FirstName = "Jane", LastName = "Smith" }
            };
            mockRepository.Setup(repo => repo.List(It.IsAny<QueryOptions<Teacher>>()))
                          .Returns(expectedTeachers);

            var controller = new TeacherController(mockRepository.Object);

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);

            var model = result.Model as List<Teacher>;
            Assert.NotNull(model);
            Assert.Equal(expectedTeachers, model);

        }

    }
}