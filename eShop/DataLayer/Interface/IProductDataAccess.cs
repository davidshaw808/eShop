using Common;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IProductDataAccess : IUnitOfWorkCUD<Product>, IAtomicCRUD<Product>
{
    public Product? Get(Guid Key);
    public IEnumerable<Product>? GetAll(IEnumerable<Guid> Keys);
    public bool UpdateAll(IEnumerable<Product> products);
}
