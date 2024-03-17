using Common.Interface;
using System.Text.Json.Serialization;

namespace Common
{
    public class Product : IElement<Product>, ISecureElement
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int? NumberInStock { get; set; }
        public int? RestrictedToAge { get; set; } 
        public Category? Category { get; set; }
        public IList<Review>? Reviews { get; set; }
        

        public Product Visit(IVisitor<Product> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
