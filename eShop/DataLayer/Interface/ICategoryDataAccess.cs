using Common;
using Common.Models.Mutable;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface ICategoryDataAccess : IUnitOfWorkCRUD<Category>, IAtomicCRUD<Category>
{
    Task AddChildAtomicAsync(Category parent, Category child);
    Task UpdateAtomicAsync(IEnumerable<Category> categories);
}
