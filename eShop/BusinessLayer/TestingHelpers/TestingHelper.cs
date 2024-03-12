using BusinessLayer.Implementation;
using BusinessLayer.Implementation.Admin;
using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using DataLayer.Databases.Base;
using DataLayer.Implementation;
using DataLayer.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer.TestingHelpers
{
    public static class TestingHelper
    {
        private static IServiceProvider? ServiceProvider { get; set; } = null;

        private static ServiceProvider BuildTestServices(eShopBaseContext db)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAddressDataAccess>(provider => new AddressDataAccess(db))
                .AddSingleton<ICategoryDataAccess>(provider => new CategoryDataAccess(db))
                .AddSingleton<ICustomerDataAccess>(provider => new CustomerDataAccess(db))
                .AddSingleton<IOrderDataAccess>(provider => new OrderDataAccess(db))
                .AddSingleton<IProductDataAccess>(provider => new ProductDataAccess(db))
                .AddSingleton<IRefundDataAccess>(provider => new RefundDataAccess(db));


            services.AddSingleton<IAddressService, AddressService>()
                .AddSingleton<IAddressOrderService, AddressOrderService>()
                .AddSingleton<ICategoryServiceAdmin, CategoryService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<ICustomerOrderService, CustomerOrderService>()
                .AddSingleton<ICustomerOrderServiceAdmin, CustomerOrderService>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IOrderServiceAdmin, OrderServiceAdmin>()
                .AddSingleton<IOrderCustomerServiceAdmin, OrderCustomerServiceAdmin>()
                .AddSingleton<IProductServiceAdmin, ProductServiceAdmin>()
                .AddSingleton<IProductOrderServiceAdmin, ProductOrderServiceAdmin>();

            return services.BuildServiceProvider();
        }

        public static T GetService<T>(eShopBaseContext db) where T : class
        {
            if(ServiceProvider == null)
            {
                ServiceProvider = BuildTestServices(db);
            }

            return ServiceProvider.GetRequiredService<T>();
        }

    }
}
