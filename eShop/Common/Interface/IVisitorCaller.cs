using Common.Models.Immutable;
using Common.Models.Mutable;

namespace Common.Interface;

public interface IVisitorCaller : 
    IVisitor<Address, Address>, 
    IVisitor<AdminUser, AdminUser>,
    IVisitor<Category, Category>,
    IVisitor<Customer, Customer>,
    IVisitor<FinancialTransaction, FinancialTransaction>,
    IVisitor<HistoryLog, HistoryLog>,
    IVisitor<Order, Order>,
    IVisitor<Product, Product>,
    IVisitor<Review, Review>
{
}

public interface IVisitorCaller<T> :
    IVisitor<Address, T>, 
    IVisitor<AdminUser, T>,
    IVisitor<Category, T>,
    IVisitor<Customer, T>,
    IVisitor<FinancialTransaction, T>,
    IVisitor<HistoryLog, T>,
    IVisitor<Order, T>,
    IVisitor<Product, T>,
    IVisitor<Review, T>
{ 
}
