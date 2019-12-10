using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace User.API.UnitTest
{
    public class UsersControllerTest
    {
        private Data.UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<Data.UserContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var userContext = new Data.UserContext(options);

            userContext.Users.Add(new Entity.Models.AppUser
            {
                Name = "lmj",
                Age = 18
            });

            userContext.SaveChanges();
            return userContext;
        }

        [Fact]
        public async Task Get_ReturnRightUser_With_ExpectedParameters()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<Controllers.UsersController>>();
            var logger = loggerMoq.Object;
            var controller = new Controllers.UsersController(context, logger, null, null, null);

            var response = await controller.Get();
            Assert.IsType<OkObjectResult>(response);
        }
    }
}
