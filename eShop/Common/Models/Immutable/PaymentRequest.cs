using Common.Base;
using Common.Interface;

namespace Common.Models.Immutable;

public record PaymentRequest : IElementImmutable<PaymentRequest>
{
    public int Id { get; init; }
    public decimal AmountNet { get; set; }
    public DateTime DateGenerated { get; set; }
    public Person Requester { get; set; }
    public Person Approver { get; set; }
    public bool Active { get; set; }
    public Guid Key { get; init; }
    public bool Credit { get; set; }

    public U Visit<U>(IVisitor<PaymentRequest, U> visitor)
    {
        return visitor.Visit(this);
    }
}
