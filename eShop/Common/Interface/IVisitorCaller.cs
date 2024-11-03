namespace Common.Interface;

public interface IVisitorCaller<A, AU, C, CU, FT, H, O, P, R> : 
    IVisitor<Address, A>, 
    IVisitor<AdminUser, AU>,
    IVisitor<Category, C>,
    IVisitor<Customer, CU>,
    IVisitor<FinancialTransaction, FT>,
    IVisitor<HistoryLog, H>,
    IVisitor<Order, O>,
    IVisitor<Product, P>,
    IVisitor<Review, R>
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
