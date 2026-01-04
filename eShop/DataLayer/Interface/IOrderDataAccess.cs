using Common.Models.Mutable;
using DataLayer.Interface.General;
namespace DataLayer.Interface;

public interface IOrderDataAccess : IUnitOfWorkCRUD<Order>, IAtomicCRUD<Order>
{
    IAsyncEnumerable<Order> GetOrdersAtomicAsync(Guid CustKey);
}
