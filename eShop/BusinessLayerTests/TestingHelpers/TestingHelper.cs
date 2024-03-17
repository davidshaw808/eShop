using BusinessLayer.Implementation.Admin;
using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using DataLayer.Databases.Base;
using DataLayer.Implementation;
using DataLayer.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayerTests.TestingHelpers
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
                .AddSingleton<IFinancialTransaction>(provider => new FinancialTransaction(db))
                .AddSingleton<IReviewDataAccess>(provider => new ReviewDataAccess(db));


            services.AddSingleton<IAddressService, AddressService>()
                .AddSingleton<IAddressOrderService, AddressOrderService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<ICustomerOrderService, CustomerOrderServiceAdmin>()
                .AddSingleton<ICustomerOrderServiceAdmin, CustomerOrderServiceAdmin>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<ICategoryServiceAdmin, CategoryService>()

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
