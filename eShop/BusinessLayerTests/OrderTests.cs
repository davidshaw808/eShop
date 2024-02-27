using BusinessLayer.Interface;
using BusinessLayer.TestingHelpers;
using Common;
using Common.Enum;
using DataLayer.Databases;

namespace BusinessLayerTests
{
    [TestClass]
    public class OrderTests
    {
        private readonly InMemoryContext _db;

        public OrderTests() {
            this._db = new InMemoryContext();
            _db.Database.EnsureCreated();
        }

        [TestMethod]
        public void GenerateOrder()
        {
            //arrange
            var addr = new Address() { HouseNameNumber = "22", PostalCode = "test" };
            var cust = new Customer() { Address = addr, Email = "test@test" };
            var pd = new PaymentDetails()
            {
                Amount = (decimal)100.01,
                Currency = Currency.GBP,
                PaymentProvider = PaymentProvider.Paypal,
                Created = DateTime.UtcNow,
                PaymentProviderId = Guid.NewGuid().ToString()
            };
            var order = new Order()
            {
                Active = true,
                Address = addr,
                Amount = (decimal)100.01,
                Currency = Common.Enum.Currency.GBP,
                Customer = cust,
                PaymentDetails = pd
            };
            var os = TestingHelper.GetService<IOrderService>(this._db);
            //act
            var result = os.Generate(order);
            //assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UpdateOrder()
        {
            //arrange
            var deliveredToCourier = "Delivered to courier";
            var productservice = TestingHelper.GetService<IProductService>(this._db);
            var product = new Product()
            {
                Active = true,
                Name = "Product one",
                Description = "Fluted glass jar",
                Price = (decimal)33.50,
                NumberInStock = 5
            };
            var product2 = new Product()
            {
                Active = true,
                Name = "Product two",
                Description = "Glass candle holder",
                Price = (decimal)22.99,
                NumberInStock = 10
            };
            productservice.Generate(product);
            productservice.Generate(product2);

            var addr = new Address() { HouseNameNumber = "The Red Lion", PostalCode = "N21 8NQ" };
            var basket = new List<Product>() { product, product2 };
            var cust = new Customer() { Address = addr, Email = "noreply@theredlion.co.uk", Basket = basket };
            var paymentdetails = new PaymentDetails()
            {
                Amount = (decimal)33.50,
                Currency = Currency.GBP,
                PaymentProvider = PaymentProvider.Paypal,
                Created = DateTime.UtcNow,
                PaymentProviderId = Guid.NewGuid().ToString()
            };
            var order = new Order()
            {
                Active = true,
                Address = addr,
                Amount = (decimal)56.49,
                Currency = Common.Enum.Currency.GBP,
                Customer = cust,
                PaymentDetails = paymentdetails
            };
            //act
            var orderservice = TestingHelper.GetService<IOrderService>(this._db);
            orderservice.Generate(order);
            Assert.IsNotNull(order.AltId);
            var orderId = order.AltId;
            var returnedOrder = orderservice.GetOrder((Guid)orderId);
            Assert.IsNotNull(returnedOrder);
            orderservice.AddOrderUpdate(returnedOrder, new OrderUpdate() { 
                Status = OrderStatus.InTransit,
                UpdateText = deliveredToCourier
            });
            orderservice.Update(returnedOrder);
            returnedOrder = orderservice.GetOrder((Guid)orderId);
            //assert
            Assert.AreEqual(returnedOrder?.Updates?.LastOrDefault()?.UpdateText ?? string.Empty, deliveredToCourier);
        }

        ~OrderTests() {
            if (this._db != null)
            { 
                this._db.Dispose(); 
            }
        }
    }
}