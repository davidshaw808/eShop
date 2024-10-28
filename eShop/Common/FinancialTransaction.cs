using Common.Enum;
using Common.Interface;

namespace Common;

public class FinancialTransaction : IElement<FinancialTransaction>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    virtual public bool Credit { get; set; }
    public Order Order { get; set; }
    public string Description { get; set; }
    public decimal? Amount { get; set; }
    public bool Active { get; set; }
    public DateTime DateGenerated { get; set; }
    public DateTime? DateApproved { get; set; }
    public DateTime? DatePaid { get; set; }
    public string? jsonPaymentProviderResponse {  get; set; }
    public PaymentProvider PaymentProvider { get; set; }

    public U Visit<U>(IVisitor<FinancialTransaction, U> visitor)
    {
        return visitor.Visit(this);
    }
}

public class RefundRequest : FinancialTransaction 
{
    public override bool Credit { get => base.Credit; }
    private RefundRequest()
    {
        base.Credit = false;
    }
    public static FinancialTransaction GenerateRefund()
    {
        return new RefundRequest();
    }
}

public class PaymentRequest : FinancialTransaction 
{
    public override bool Credit { get => base.Credit; }
    private PaymentRequest()
    {
        base.Credit = true;
    }
    public static FinancialTransaction GeneratePayment()
    {
        return new PaymentRequest();
    }
}
