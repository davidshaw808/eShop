using Common.Models.Immutable;
using Common.Models.Mutable;

namespace Common.Interface;

public interface IVisitorCaller<Adr, AdUs, Cat, Cust, FT, Hist, Ord, Prod, Rev> : 
    IVisitor<Address, Adr>, 
    IVisitor<AdminUser, AdUs>,
    IVisitor<Category, Cat>,
    IVisitor<Customer, Cust>,
    IVisitor<FinancialTransaction, FT>,
    IVisitor<HistoryLog, Hist>,
    IVisitor<Order, Ord>,
    IVisitor<Product, Prod>,
    IVisitor<Review, Rev>
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
