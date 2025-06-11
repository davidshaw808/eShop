using Common.Interface;
using Common.Models.Immutable;

namespace BusinessLayer.Interface.Admin;

public interface IRefundServiceAdmin : IGenerateUpdateDelete<FinancialTransaction>
{
    public bool ApproveRefund(Guid refundId);
    public bool RejectRefund(Guid refundId);
    public IEnumerable<FinancialTransaction> GetAllPendingApproval();
    public IEnumerable<FinancialTransaction> GetAllRequiringPayment();
}
