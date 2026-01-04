using Common.Enum;
using Common.Interface;
using Common.Models.Mutable;

namespace Common.Models.Immutable;

public sealed record FinancialTransaction : IElementImmutable<FinancialTransaction>
{
    public int Id { get; init; }
    public Guid Key { get; init; }
    public bool Credit { get => this.PaymentType.Equals(PaymentType.Refund); }
    public Order Order { get; set; }
    public PaymentType PaymentType { get; set; }
    public PaymentRequest PaymentRequest { get; set; }
    public string Description { get; set; }
    public decimal AmountNet { get; set; }
    public Currency Currency { get; set; }
    public bool Active { get; set; }
    public DateTime DatePaid { get; set; }
    public string JsonPaymentProviderResponse { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
    public DateTime Created { get; set; }
    public Vat Vat { get; set; }

    public U Visit<U>(IVisitor<FinancialTransaction, U> visitor)
    {
        return visitor.Visit(this);
    }
}
