using Common.Enum;
using Common.Interface;

namespace Common.Models.Immutable;

public record Vat : IElement<Vat>
{
    public int? Id { get; set; }
    public Currency Currency { get; set; }
    public decimal Rate { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool Active { get; set; }
    public Guid? Key { get; set; }

    public U Visit<U>(IVisitor<Vat, U> visitor)
    {
        return visitor.Visit(this);
    }
    //TODO ask if other countries rates will be needed
}
