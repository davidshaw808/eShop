using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface ICategoryDataAccess : IGenerateUpdateDelete<Category>
    {
        Task<int> AddChildAsync(Category p, Category c);
        Task<Category?> GetAsync(int id);
        Task<int> UpdateAsync(IEnumerable<Category> cats);
        int Update(IEnumerable<Category> cats, bool commit);
        Task<int> LogicalDeleteAsync(int id);
    }
}
