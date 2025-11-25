using Microsoft.EntityFrameworkCore;
using StockLine_API;
using StockLine_API.Models;
using StockLine_API.Services;
using StockLine_API.DTOs;
using Xunit;

namespace StockLineAPI_Tests
{
    public class MovimientoStockServiceTests
    {
        private StockLineContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StockLineContext>()
                .UseInMemoryDatabase(databaseName: "MovimientoStockServiceTestDb")
                .Options;
            return new StockLineContext(options);
        }

   

        [Fact]
        public void Create_Salida_InsufficientStock_ReturnsNull()
        {
            var context = GetInMemoryDbContext();
            var producto = new Producto { Nombre = "Prod1", Stock = 1 };
            context.Productos.Add(producto);
            var usuario = new Usuario { Nombre = "User", Apellidos = "Test", Email = "user@email.com", Activo = true, PasswordHash = "hash" };
            context.Usuarios.Add(usuario);
            context.SaveChanges();
            var service = new MovimientoStockService(context);
            var dto = new MovimientoStockDTO { ProductoID = producto.ProductoID, Cantidad = 5, TipoMovimiento = "Salida", UsuarioID = usuario.UsuarioID, Observaciones = "Test" };
            var (mov, stockAfter) = service.Create(dto);
            Assert.Null(mov);
            Assert.Equal(1, context.Productos.First().Stock);
        }
    }
}
