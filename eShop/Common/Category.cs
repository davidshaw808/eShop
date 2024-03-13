using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Category : IElement<Category>
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public string? Name { get; set; }
        public Category? Parent { get; set; }
        public IList<Category>? Children { get; set;}
        public IList<Product>? Products { get; set;}
        public bool Active { get; set; }

        public Category Visit(IVisitor<Category> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
