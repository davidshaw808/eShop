using Common.Interface;
using Common;

namespace BusinessLayer.Interface.Admin;

public interface IRefundServiceAdmin : IGenerateUpdateDelete<FinancialTransaction>
{
    public bool ApproveRefund(Guid refundId);
    public bool RejectRefund(Guid refundId);
    public IEnumerable<FinancialTransaction> GetAllPendingApproval();
    public IEnumerable<FinancialTransaction> GetAllRequiringPayment();
}
