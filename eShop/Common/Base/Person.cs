using Common.Interface;

namespace Common.Base;

public abstract class Person : IElementId, IElementKey
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }

    public Address Address { get; set; }

    public IEnumerable<Order>? OrderHistory { get; set; }
    public IEnumerable<Product>? Basket { get; set; }
    public bool LockBasket { get; set; }
    public bool Active { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
