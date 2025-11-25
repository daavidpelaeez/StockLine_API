using Microsoft.EntityFrameworkCore;
using StockLine_API;
using StockLine_API.Models;
using StockLine_API.Services;
using Xunit;

namespace StockLineAPI_Tests
{
    public class ProductoServiceTests
    {
        private StockLineContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StockLineContext>()
                .UseInMemoryDatabase(databaseName: "ProductoServiceTestDb")
                .Options;
            return new StockLineContext(options);
        }

        [Fact]
        public void Create_AddsProducto()
        {
            var context = GetInMemoryDbContext();
            var service = new ProductoService(context);
            var producto = new Producto { Nombre = "Prod1", Descripcion = "Desc", Stock = 10, CategoriaID = 1 };
            var result = service.Create(producto);
            Assert.NotNull(result);
            Assert.Equal("Prod1", result.Nombre);
            Assert.Single(context.Productos);
        }

        [Fact]
        public void Get_ReturnsProducto()
        {
            var context = GetInMemoryDbContext();
            var producto = new Producto { Nombre = "Prod1", Descripcion = "Desc", Stock = 10, CategoriaID = 1 };
            context.Productos.Add(producto);
            context.SaveChanges();
            var service = new ProductoService(context);
            var result = service.Get(producto.ProductoID);
            Assert.NotNull(result);
            Assert.Equal("Prod1", result.Nombre);
        }

    }
}
