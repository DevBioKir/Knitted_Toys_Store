using Knitted_Toys_Store.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace Knitted_Toys_Store.UnitTest.Test
{
    public abstract class TestBase : IDisposable
    {
        protected Knitted_Toys_StoreDBContext _dbContext {  get; private set; }

        public TestBase()
        {
            var options = new DbContextOptionsBuilder<Knitted_Toys_StoreDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new Knitted_Toys_StoreDBContext(options);
            SeedData();
        }

        /// <summary>
        /// Метод для инициализации данных в тестовой базе
        /// Можно переопределить в наследуемых классах
        /// </summary>
        protected virtual void SeedData() { }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
