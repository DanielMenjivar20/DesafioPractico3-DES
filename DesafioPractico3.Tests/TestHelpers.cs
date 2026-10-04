using DesafioPractico3.Data;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DesafioPractico3.Tests
{
    public static class TestHelpers
    {
        // Cada prueba usa su propia base en memoria (nombre único)
        public static ApplicationDbContext CrearContexto()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        public static Mock<IOutputCacheStore> CrearCacheMock() => new Mock<IOutputCacheStore>();
    }
}