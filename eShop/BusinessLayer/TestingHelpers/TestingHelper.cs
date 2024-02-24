﻿using BusinessLayer.Implementation;
using BusinessLayer.Interface;
using DataLayer.Databases.Base;
using DataLayer.Implementation;
using DataLayer.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer.TestingHelpers
{
    public static class TestingHelper
    {
        private static IServiceProvider? ServiceProvider { get; set; } = null;

        private static ServiceProvider BuildTestServices(CrispHabitatBaseContext db)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IAddressDataAccess>(provider => new AddressDataAccess(db))
                .AddSingleton<ICategoryDataAccess>(provider => new CategoryDataAccess(db))
                .AddSingleton<ICustomerDataAccess>(provider => new CustomerDataAccess(db))
                .AddSingleton<IOrderDataAccess>(provider => new OrderDataAccess(db))
                .AddSingleton<IProductDataAccess>(provider => new ProductDataAccess(db))
                .AddSingleton<IRefundDataAccess>(provider => new RefundDataAccess(db));


            services.AddSingleton<IAddressService, AddressService>()
                .AddSingleton<ICategoryService, CategoryService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IOrderService, OrderService>()
                .AddSingleton<IProductService, ProductService>();

            return services.BuildServiceProvider();
        }

        public static T GetService<T>(CrispHabitatBaseContext db) where T : class
        {
            if(ServiceProvider == null)
            {
                ServiceProvider = BuildTestServices(db);
            }

            return ServiceProvider.GetRequiredService<T>();
        }

    }
}
