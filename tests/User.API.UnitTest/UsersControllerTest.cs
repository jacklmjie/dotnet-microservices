using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
using System.Linq;

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

        private (Controllers.UsersController, Data.UserContext) GetUsersController()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<Controllers.UsersController>>();
            var logger = loggerMoq.Object;
            var controller = new Controllers.UsersController(context, logger, null, null, null);
            return (controller, context);
        }

        [Fact]
        public async Task Get_ReturnRightUser_With_ExpectedParameters()
        {

            (var controller, var userContext) = GetUsersController();
            var response = await controller.Get();
            var result = response.Should().BeOfType<OkObjectResult>().Subject;
            var appUser = result.Value.Should().BeOfType<Entity.Models.AppUser>().Subject;
            appUser.Id.Should().Be(1);
            appUser.Name.Should().Be("lmj");
        }

        [Fact]
        public async Task Patch_ReturnNewName_With_ExpectedNewNameParameters()
        {
            (var controller, var userContext) = GetUsersController();
            var document = new JsonPatchDocument<Entity.Models.AppUser>();
            document.Replace(u => u.Name, "li");
            var response = await controller.Patch(document);
            var result = response.Should().BeOfType<OkObjectResult>().Subject;

            //assert response
            var appUser = result.Value.Should().BeAssignableTo<Entity.Models.AppUser>().Subject;
            appUser.Name.Should().Be("li");

            //assert name value of context
            var userModel = await userContext.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Should().NotBeNull();
            userModel.Name.Should().Be("li");
        }

        [Fact]
        public async Task Patch_ReturnNewProperties_With_AddNewProperty()
        {
            (var controller, var userContext) = GetUsersController();
            var document = new JsonPatchDocument<Entity.Models.AppUser>();
            document.Replace(u => u.Properties, new List<Entity.Models.UserProperty> {
                new Entity.Models.UserProperty()
                {
                    Key="fin_industry",
                    Text="»¥ÁªÍø",
                    Value="»¥ÁªÍø"
                }
            });
            var response = await controller.Patch(document);
            var result = response.Should().BeOfType<OkObjectResult>().Subject;

            //assert response
            var appUser = result.Value.Should().BeAssignableTo<Entity.Models.AppUser>().Subject;
            appUser.Properties.Count.Should().Be(1);
            appUser.Properties.First().Key.Should().Be("fin_industry");
            appUser.Properties.First().Text.Should().Be("»¥ÁªÍø");

            //assert name value of context
            var userModel = await userContext.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Count.Should().Be(1);
            userModel.Properties.First().Key.Should().Be("fin_industry");
            userModel.Properties.First().Text.Should().Be("»¥ÁªÍø");
        }

        [Fact]
        public async Task Patch_ReturnNewProperties_With_RemoveProperty()
        {
            (var controller, var userContext) = GetUsersController();
            var document = new JsonPatchDocument<Entity.Models.AppUser>();
            document.Replace(u => u.Properties, new List<Entity.Models.UserProperty> {});
            var response = await controller.Patch(document);
            var result = response.Should().BeOfType<OkObjectResult>().Subject;

            //assert response
            var appUser = result.Value.Should().BeAssignableTo<Entity.Models.AppUser>().Subject;
            appUser.Properties.Should().BeEmpty();

            //assert name value of context
            var userModel = await userContext.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Should().BeEmpty();
        }
    }
}
