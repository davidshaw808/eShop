using Common;
using Common.Interface;
using DataLayer.Interface.General;

namespace DataLayer.Interface
{
    public interface ICategoryDataAccess : IUnitOfWorkCUD<Category>, IAtomicCRUD<Category>
    {
        Task<int> AddChildAsync(Category p, Category c);
        int Update(IEnumerable<Category> cats);
    }
}
