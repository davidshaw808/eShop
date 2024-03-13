using Common.Interface;
using System.ComponentModel.DataAnnotations;

namespace Common.Base
{
    public abstract class Person : IElement<Person>, ISecureElement
    {
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }

        public Address? Address { get; set; }

        public IList<Order>? OrderHistory { get; set; }
        public IList<Product>? Basket { get; set; }
        public bool Active {  get; set; }

        public abstract Person Visit(IVisitor<Person> visitor);
    }
}
