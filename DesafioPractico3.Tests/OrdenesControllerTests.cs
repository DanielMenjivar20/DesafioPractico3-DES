using DesafioPractico3.Controllers;
using DesafioPractico3.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DesafioPractico3.Tests
{
    public class OrdenesControllerTests
    {
        [Fact]
        public async Task PostOrden_Valida_DevuelveCreatedEInvalidaCache()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            context.Clientes.Add(new Cliente { Id = 1, Nombre = "Ana", Email = "ana@email.com" });
            await context.SaveChangesAsync();
            var cache = TestHelpers.CrearCacheMock();
            var controller = new OrdenesController(context, cache.Object);

            // Act
            var result = await controller.PostOrden(new Orden { ClienteId = 1, MontoTotal = 150.50m });

            // Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(1, context.Ordenes.Count());
            cache.Verify(c => c.EvictByTagAsync("ordenes", It.IsAny<CancellationToken>()), Times.Once);
            cache.Verify(c => c.EvictByTagAsync("clientes", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task PostOrden_MontoCero_DevuelveBadRequest()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            context.Clientes.Add(new Cliente { Id = 1, Nombre = "Ana", Email = "ana@email.com" });
            await context.SaveChangesAsync();
            var controller = new OrdenesController(context, TestHelpers.CrearCacheMock().Object);

            // Act
            var result = await controller.PostOrden(new Orden { ClienteId = 1, MontoTotal = 0 });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Empty(context.Ordenes);
        }

        [Fact]
        public async Task PostOrden_ClienteInexistente_DevuelveBadRequest()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            var controller = new OrdenesController(context, TestHelpers.CrearCacheMock().Object);

            // Act
            var result = await controller.PostOrden(new Orden { ClienteId = 99, MontoTotal = 50m });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetOrdenesPorCliente_DevuelveSoloLasDelCliente()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            context.Clientes.AddRange(
                new Cliente { Id = 1, Nombre = "Ana", Email = "ana@email.com" },
                new Cliente { Id = 2, Nombre = "Luis", Email = "luis@email.com" });
            context.Ordenes.AddRange(
                new Orden { ClienteId = 1, MontoTotal = 10m },
                new Orden { ClienteId = 1, MontoTotal = 20m },
                new Orden { ClienteId = 2, MontoTotal = 30m });
            await context.SaveChangesAsync();
            var controller = new OrdenesController(context, TestHelpers.CrearCacheMock().Object);

            // Act
            var result = await controller.GetOrdenesPorCliente(1);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var ordenes = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.Equal(2, ordenes.Count());
        }

        [Fact]
        public async Task GetOrden_IdInexistente_DevuelveNotFound()
        {
            // Arrange
            using var context = TestHelpers.CrearContexto();
            var controller = new OrdenesController(context, TestHelpers.CrearCacheMock().Object);

            // Act
            var result = await controller.GetOrden(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
    }
}