using Common.Models.Mutable;

namespace BusinessLayer.ClassHelpers;

public class GroupedProduct
{
    public int Id { get; set; }
    public IEnumerable<Product>? Products { get; set; }
}
