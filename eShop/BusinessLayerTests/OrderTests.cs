using BusinessLayer.Implementation.Admin;
using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using BusinessLayerTests.TestingHelpers;
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
        public void GenerateOrderAdmin()
        {
            var productservice = TestingHelper.GetService<IProductServiceAdmin>(this._db);
            var orderService = TestingHelper.GetService<IOrderServiceAdmin>(this._db);
            var customerService = TestingHelper.GetService<ICustomerService>(this._db);
            var addressService = TestingHelper.GetService<IAddressService>(this._db);
            var numberInStock = 10;
            //arrange
            var product = new Product()
            {
                Active = true,
                Name = "Bowl",
                Description = "Clay bowl",
                Price = (decimal)5.99,
                NumberInStock = numberInStock
            };
            productservice.Generate(product);
            var addr = new Address() { Active = true, HouseNameNumber = "22", PostalCode = "test" };
            addressService.Generate(addr);
            var cust = new Customer() { Active = true, Address = addr, Email = "test@test", Basket = [product] };
            customerService.Generate(cust);
            //act
            var canProcess = orderService.CanProcessBasket((Guid)cust.AltId);
            Assert.IsFalse(canProcess.IsError);
            var result = orderService.Generate(cust, "0123456", "{ id:0123456, paymentDate:'12/3/24' payment:5.99 }", (decimal)5.99, Currency.GBP, PaymentProvider.Paypal, addr);
            //assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.NumberInStock, numberInStock-1);
        }

        [TestMethod]
        public void GenerateOrderWithRefundAdmin()
        {
            var productservice = TestingHelper.GetService<IProductServiceAdmin>(this._db);
            var orderService = TestingHelper.GetService<IOrderServiceAdmin>(this._db);
            var customerService = TestingHelper.GetService<ICustomerService>(this._db);
            var addressService = TestingHelper.GetService<IAddressService>(this._db);
            var numberInStock = 10;
            //arrange
            var product = new Product()
            {
                Active = true,
                Name = "Bowl",
                Description = "Clay bowl",
                Price = (decimal)5.99,
                NumberInStock = numberInStock
            };
            productservice.Generate(product);
            var addr = new Address() { Active = true, HouseNameNumber = "22", PostalCode = "test" };
            addressService.Generate(addr);
            var cust = new Customer() { Active = true, Address = addr, Email = "test@test", Basket = [product] };
            customerService.Generate(cust);
            //act
            var canProcess = orderService.CanProcessBasket((Guid)cust.AltId);
            Assert.IsFalse(canProcess.IsError);
            var result = orderService.Generate(cust, "0123456", "{ id:0123456, paymentDate:'12/3/24' payment:11.99 }", (decimal)11.98, Currency.GBP, PaymentProvider.Paypal, addr);
            //assert
            Assert.IsNotNull(result);
            var order = orderService.GetOrder((Guid)result);
            Assert.AreEqual(product.NumberInStock, numberInStock - 1);
            Assert.AreEqual(order.Refunds.First().Amount, (decimal)5.99);
        }

        [TestMethod]
        public void UpdateOrderAdmin()
        {
            var productservice = TestingHelper.GetService<IProductServiceAdmin>(this._db);
            var orderservice = TestingHelper.GetService<IOrderServiceAdmin>(this._db);
            var addressService = TestingHelper.GetService<IAddressService>(this._db);
            var customerService = TestingHelper.GetService<ICustomerService>(this._db);
            //arrange
            var deliveredToCourier = "Delivered to courier";
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
            addressService.Generate(addr);
            var basket = new List<Product>() { product, product2 };
            var cust = new Customer() { Address = addr, Email = "noreply@theredlion.co.uk", Basket = basket };
            customerService.Generate(cust);
            //act
            var orderId = orderservice.Generate(cust, "0123456", "{ id:0123456, paymentDate:'12/3/24' payment:89.99 }", (decimal)56.49, Currency.GBP, PaymentProvider.Paypal, addr);
            Assert.IsNotNull(orderId);
            var returnedOrder = orderservice.GetOrder((Guid)orderId);
            Assert.IsNotNull(returnedOrder);
            orderservice.AddOrderUpdate(returnedOrder, new OrderUpdate() { 
                Status = OrderStatus.InTransit,
                UpdateText = deliveredToCourier
            });
            orderservice.Update(returnedOrder);
            returnedOrder = orderservice.GetOrder((Guid)orderId);
            //assert
            var updateText = returnedOrder?.Updates?.LastOrDefault()?.UpdateText ?? string.Empty;
            Assert.AreEqual(updateText, deliveredToCourier);
            Assert.AreEqual(product.NumberInStock, 4);
            Assert.AreEqual(product2.NumberInStock, 9);
        }
        
        [TestMethod]
        public void UpdateOrderCustomer()
        {
            var productservice = TestingHelper.GetService<IProductServiceAdmin>(this._db);
            var customerService = TestingHelper.GetService<ICustomerService>(this._db);
            var orderService = TestingHelper.GetService<IOrderService>(this._db);
            //arrange
            var product = new Product()
            {
                Active = true,
                Name = "Ceramic Jug",
                Description = "Jug with intricate detail",
                Price = (decimal)12.50,
                NumberInStock = 3
            };
            var product2 = new Product()
            {
                Active = true,
                Name = "Tablecloth",
                Description = "Detailed woven tableware",
                Price = (decimal)19.99,
                NumberInStock = 5
            };
            productservice.Generate(product);
            productservice.Generate(product2);
            var addr = new Address() { HouseNameNumber = "22", PostalCode = "HU5 1LN" };
            var cust = new Customer() { Address = addr, Email = "noreply@visithull.co.uk" };
            customerService.Generate(cust);

            Assert.IsNotNull(cust.AltId);
            customerService.AddItemToBasket((Guid)cust.AltId, product);
            customerService.AddItemToBasket((Guid)cust.AltId, product);
            customerService.AddItemToBasket((Guid)cust.AltId, product);
            customerService.AddItemToBasket((Guid)cust.AltId, product2);
            customerService.AddItemToBasket((Guid)cust.AltId, product2);
            customerService.AddItemToBasket((Guid)cust.AltId, product2);
            customerService.AddItemToBasket((Guid)cust.AltId, product2);
            var canProcess = orderService.CanProcessBasket((Guid)cust.AltId);
            Assert.IsFalse(canProcess.IsError);
            customerService.AddItemToBasket((Guid)cust.AltId, product);
            canProcess = orderService.CanProcessBasket((Guid)cust.AltId);
            Assert.IsTrue(canProcess.IsError);
            Assert.IsTrue(canProcess.Message.Length > 0);
        }

        ~OrderTests() {
            if (this._db != null)
            { 
                this._db.Dispose(); 
            }
        }
    }
}