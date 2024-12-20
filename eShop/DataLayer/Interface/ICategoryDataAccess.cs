using Common;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface ICategoryDataAccess : IUnitOfWorkCUD<Category>, IAtomicCRUD<Category>
{
    ValueTask<int> AddChildAtomicAsync(Category parent, Category child);
    ValueTask<int> UpdateAtomicAsync(IEnumerable<Category> categories);
    void AssignChildenToParent(Category category);
}
