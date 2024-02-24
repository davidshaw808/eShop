using Common.Interface;

namespace Common.Base
{
    public abstract class Person : IElement<Person>, ISecureElement
    {
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }

        public Address? Address { get; set; }

        public IList<Order>? OrderHistory { get; set; }
        public IList<Product>? Basket { get; set; }
        public bool Active {  get; set; }

        public abstract S Visit<S>(Func<Person, S> visitor);
    }
}
