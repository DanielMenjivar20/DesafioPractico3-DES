using DesafioPractico3.Controllers;
using DesafioPractico3.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DesafioPractico3.Tests
{
    public class ClientesControllerTests
    {
        [Fact]
        public async Task PostCliente_ConDatosValidos_DevuelveCreatedYGuarda()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            var cache = TestHelpers.CrearCacheMock();
            var controller = new ClientesController(context, cache.Object);
            var cliente = new Cliente { Nombre = "Ana", Email = "ana@email.com" };

            // Act
            var result = await controller.PostCliente(cliente);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, context.Clientes.Count());
            Assert.NotEqual(default, context.Clientes.First().FechaRegistro);
            cache.Verify(c => c.EvictByTagAsync("clientes", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetCliente_IdInexistente_DevuelveNotFound()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            var controller = new ClientesController(context, TestHelpers.CrearCacheMock().Object);

            // Act
            var result = await controller.GetCliente(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCliente_Existente_DevuelveElCliente()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            context.Clientes.Add(new Cliente { Id = 1, Nombre = "Luis", Email = "luis@email.com" });
            await context.SaveChangesAsync();
            var controller = new ClientesController(context, TestHelpers.CrearCacheMock().Object);

            // Act
            var result = await controller.GetCliente(1);

            // Assert
            Assert.Equal("Luis", result.Value!.Nombre);
        }

        [Fact]
        public async Task PutCliente_IdNoCoincide_DevuelveBadRequest()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            var controller = new ClientesController(context, TestHelpers.CrearCacheMock().Object);
            var cliente = new Cliente { Id = 2, Nombre = "X", Email = "x@email.com" };

            // Act
            var result = await controller.PutCliente(1, cliente);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteCliente_Existente_EliminaYDevuelveNoContent()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            context.Clientes.Add(new Cliente { Id = 1, Nombre = "Eva", Email = "eva@email.com" });
            await context.SaveChangesAsync();
            var cache = TestHelpers.CrearCacheMock();
            var controller = new ClientesController(context, cache.Object);

            // Act
            var result = await controller.DeleteCliente(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Clientes);
            cache.Verify(c => c.EvictByTagAsync("clientes", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}