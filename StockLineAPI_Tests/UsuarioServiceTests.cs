using Microsoft.EntityFrameworkCore;
using StockLine_API;
using StockLine_API.Models;
using StockLine_API.Services;
using Xunit;
using Moq;
using System;

namespace StockLineAPI_Tests
{
    public class UsuarioServiceTests
    {
        private static StockLineContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StockLineContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única por test
                .Options;
            return new StockLineContext(options);
        }

        [Fact]
        public void Create_NewUser_ReturnsUser()
        {
            var context = GetInMemoryDbContext();
            // Agregar Role necesario para el usuario
            context.Roles.Add(new Role { RoleID = 1, Nombre = "TestRole", Descripcion = "Test" });
            context.SaveChanges();
            var comercialService = new Mock<ComercialService>(context);
            var service = new UsuarioService(context, comercialService.Object);
            var user = new Usuario { Nombre = "Test", Apellidos = "User", Email = "test@email.com", RoleID = 1, Activo = true, PasswordHash = "hash" };
            var result = service.Create(user, "password123");
            Assert.NotNull(result);
            Assert.Equal("test@email.com", result.Email);
        }

        [Fact]
        public void Create_ExistingEmail_ReturnsNull()
        {
            var context = GetInMemoryDbContext();
            context.Roles.Add(new Role { RoleID = 1, Nombre = "TestRole", Descripcion = "Test" });
            context.Usuarios.Add(new Usuario { Nombre = "Test", Apellidos = "User", Email = "test@email.com", RoleID = 1, PasswordHash = "hash", Activo = true });
            context.SaveChanges();
            var comercialService = new Mock<ComercialService>(context);
            var service = new UsuarioService(context, comercialService.Object);
            var user = new Usuario { Nombre = "Test2", Apellidos = "User2", Email = "test@email.com", RoleID = 1, Activo = true, PasswordHash = "hash" };
            var result = service.Create(user, "password123");
            Assert.Null(result);
        }

        [Fact]
        public void SoftDelete_UserWithoutMovimientosOrEnvios_SetsInactive()
        {
            var context = GetInMemoryDbContext();
            context.Roles.Add(new Role { RoleID = 1, Nombre = "TestRole", Descripcion = "Test" });
            var user = new Usuario { Nombre = "Test", Apellidos = "User", Email = "test@email.com", RoleID = 1, Activo = true, PasswordHash = "hash" };
            context.Usuarios.Add(user);
            context.SaveChanges();
            var comercialService = new Mock<ComercialService>(context);
            var service = new UsuarioService(context, comercialService.Object);
            var result = service.SoftDelete(user.UsuarioID);
            Assert.True(result);
            Assert.False(context.Usuarios.First().Activo);
        }
    }
}
