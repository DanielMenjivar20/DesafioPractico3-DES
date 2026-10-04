using System.Reflection;
using DesafioPractico3.Controllers;
using DesafioPractico3.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace DesafioPractico3.Tests
{
    public class AuthTests
    {
        private static Mock<UserManager<IdentityUser>> CrearUserManagerMock() =>
            new Mock<UserManager<IdentityUser>>(
                Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);

        private static Mock<RoleManager<IdentityRole>> CrearRoleManagerMock() =>
            new Mock<RoleManager<IdentityRole>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);

        private static IConfiguration CrearConfig() =>
            new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "clave-de-pruebas-de-al-menos-32-caracteres-123!",
                ["Jwt:Issuer"] = "Test",
                ["Jwt:Audience"] = "TestUsers"
            }).Build();

        [Fact]
        public async Task Login_CredencialesInvalidas_DevuelveUnauthorized()
        {
            // Arrange
            var userManager = CrearUserManagerMock();
            userManager.Setup(u => u.FindByEmailAsync("x@email.com"))
                       .ReturnsAsync((IdentityUser?)null);
            var controller = new AuthController(userManager.Object, CrearRoleManagerMock().Object, CrearConfig());

            // Act
            var result = await controller.Login(new LoginDto { Email = "x@email.com", Password = "mala" });

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task Login_CredencialesValidas_DevuelveToken()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "a@email.com", Email = "a@email.com" };
            var userManager = CrearUserManagerMock();
            userManager.Setup(u => u.FindByEmailAsync("a@email.com")).ReturnsAsync(user);
            userManager.Setup(u => u.CheckPasswordAsync(user, "Test123!")).ReturnsAsync(true);
            userManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });
            var controller = new AuthController(userManager.Object, CrearRoleManagerMock().Object, CrearConfig());

            // Act
            var result = await controller.Login(new LoginDto { Email = "a@email.com", Password = "Test123!" });

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var token = ok.Value!.GetType().GetProperty("token")!.GetValue(ok.Value) as string;
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        [Fact]
        public void ReportesController_RequiereRolAdmin()
        {
            // Arrange & Act
            var atributo = typeof(ReportesController).GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            Assert.NotNull(atributo);
            Assert.Equal("Admin", atributo!.Roles);
        }
    }
}