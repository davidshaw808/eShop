using Common;
using Common.Interface;
using System.Collections.Generic;

namespace DataLayer.Interface
{
    public interface ICategoryDataAccess : IGenerateUpdateDelete<Category>
    {
        public bool AddChild(Category p, Category c);
        public Category Get(int id);

        public bool Update(IEnumerable<Category> cats);
    }
}
