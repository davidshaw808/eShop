using Common;
using Common.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;

namespace BusinessLayer.Interface
{
    public interface ICustomerService: IGenerateUpdateDelete<Customer>
    {
        public IEnumerable<Order>? GetCustomerOrderHistory(Guid id);
        public bool RemoveAllCustomerInfo(Guid id);
        public bool AddOrder(Guid id, Order order);
        public bool AddItemsToBasket(Guid id, Product product);
        public IEnumerable<Customer> GetAllCustomers();
        public IEnumerable<Customer> GetAllActiveCustomers();
        public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName);
        public IEnumerable<Customer> GetCustomerByEmail(string email);
        public IEnumerable<Address> GetCustomerAddresses(Guid altId);
        bool TransferOrderHistory(Guid currentCustomer, Customer newCustomer);
    }
}
