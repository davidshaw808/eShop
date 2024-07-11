using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common
{
    public class Category : CategoryExtern, IInternalElement<CategoryExtern>
    {
        [JsonIgnore]
        public int? Id { get; set; }
    }

    public class CategoryExtern : IElement<CategoryExtern>, IParentChild
    {
        public int? ParentId { get; set; }
        public string? Name { get; set; }
        public CategoryExtern? Parent { get; set; }
        public IList<IParentChild>? Children { get; set;}
        public IList<Product>? Products { get; set;}
        public bool Active { get; set; }
        IParentChild? IParentChild.Parent { get => Parent; set => Parent = (CategoryExtern?)value; }

        public CategoryExtern Visit(IVisitor<CategoryExtern> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
