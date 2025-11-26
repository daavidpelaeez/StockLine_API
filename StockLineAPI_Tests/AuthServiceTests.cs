using Microsoft.EntityFrameworkCore;
using StockLine_API;
using StockLine_API.Models;
using StockLine_API.Services;
using Xunit;
using Moq;

namespace StockLineAPI_Tests
{
    public class AuthServiceTests
    {
        private static StockLineContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<StockLineContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única por test
                .Options;
            return new StockLineContext(options);
        }

       
        [Fact]
        public void Register_ExistingEmail_ReturnsNull()
        {
            var context = GetInMemoryDbContext();
            context.Usuarios.Add(new Usuario { Nombre = "Test", Apellidos = "User", Email = "test@email.com", RoleID = 1, PasswordHash = "hash", Activo = true });
            context.SaveChanges();
            var service = new AuthService(context);
            var user = new Usuario { Nombre = "Test2", Apellidos = "User2", Email = "test@email.com", RoleID = 1 };
            var result = service.Register(user, "password123");
            Assert.Null(result);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsUser()
        {
            var context = GetInMemoryDbContext();
            var password = "password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            context.Usuarios.Add(new Usuario { Nombre = "Test", Apellidos = "User", Email = "test@email.com", RoleID = 1, PasswordHash = hash, Activo = true });
            context.SaveChanges();
            var service = new AuthService(context);
            var result = service.Login("test@email.com", password);
            Assert.NotNull(result);
            Assert.Equal("test@email.com", result.Email);
        }

        [Fact]
        public void Login_InvalidPassword_ReturnsNull()
        {
            var context = GetInMemoryDbContext();
            var hash = BCrypt.Net.BCrypt.HashPassword("password123");
            context.Usuarios.Add(new Usuario { Nombre = "Test", Apellidos = "User", Email = "test@email.com", RoleID = 1, PasswordHash = hash, Activo = true });
            context.SaveChanges();
            var service = new AuthService(context);
            var result = service.Login("test@email.com", "wrongpassword");
            Assert.Null(result);
        }
    }
}
