using DocumentFormat.OpenXml.Drawing.Charts;
using Knitted_Toys_Store.App.Services;
using Knitted_Toys_Store.DataAccess;
using Knitted_Toys_Store.Domain.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Knitted_Toys_Store.App;
using MapsterMapper;
using Knitted_Toys_Store.DataAccess.Repositories;
using Microsoft.Extensions.Logging;
using Knitted_Toys_Store.Infrastructure.Middleware;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Knitted_Toys_Store.DataAccess.Entities;


namespace Knitted_Toys_Store.UnitTest.Test
{
    public class CartServiceTest : TestBase
    {
        private ICartService _CartService;
        private Mock<ICartRepositories> _mockCartRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<CartIdentifierMiddleware>> _mockLogger;

        [TestInitialize]
        public void Setup()
        {
            _mockCartRepository = new Mock<ICartRepositories>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<CartIdentifierMiddleware>>();

            _CartService = new CartService(_mockCartRepository.Object, _mockMapper.Object, _mockLogger.Object);

        }
    }
}
