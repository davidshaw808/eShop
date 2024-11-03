using Common;
using Common.Interface;
using DataLayer.Interface.General;

namespace DataLayer.Interface
{
    public interface ICategoryDataAccess : IUnitOfWorkCUD<Category>, IAtomicCRUD<Category>
    {
        Task<int> AddChildAtomicAsync(Category p, Category c);
        Task<int> UpdateAtomicAsync(IEnumerable<Category> cats);
    }
}
