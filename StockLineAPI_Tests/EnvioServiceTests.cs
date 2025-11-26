using Microsoft.EntityFrameworkCore;
using StockLine_API;
using StockLine_API.Models;
using StockLine_API.Services;
using StockLine_API.DTOs;
using Xunit;
using Moq;
using System.Collections.Generic;

namespace StockLineAPI_Tests
{
    public class EnvioServiceTests
    {
        private static StockLineContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StockLineContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única por test
                .Options;
            return new StockLineContext(options);
        }

        [Fact]
        public void GetAll_ReturnsList()
        {
            var context = GetInMemoryDbContext();
            // Agregar Comercial y Ayuntamiento necesarios
            var comercial = new Comercial { ComercialID = 1, Nombre = "Com", Apellidos = "Test", Email = "c@test.com", Telefono = "123" };
            var ayuntamiento = new Ayuntamiento { AyuntamientoID = 1, Nombre = "Ayto", Activo = true };
            context.Comerciales.Add(comercial);
            context.Ayuntamientos.Add(ayuntamiento);
            context.SaveChanges();
            var movService = new Mock<MovimientoStockService>(context);
            context.Envios.Add(new Envio { AyuntamientoID = 1, ComercialID = 1, NumeroReferencia = "REF1", Estado = "Pendiente" });
            context.SaveChanges();
            var service = new EnvioService(context, movService.Object);
            var result = service.GetAll();
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public void Create_ThrowsIfNoComercial()
        {
            var context = GetInMemoryDbContext();
            var movService = new Mock<MovimientoStockService>(context);
            var service = new EnvioService(context, movService.Object);
            var dto = new CrearEnvioDTO { AyuntamientoID = 1, NumeroReferencia = "REF2", Productos = new List<CrearEnvioDetalleDTO>() };
            Assert.Throws<System.InvalidOperationException>(() => service.Create(dto, 999));
        }
    }
}
