using Common.Interface;
using Common;

namespace BusinessLayer.Interface.Admin
{
    public interface IRefundServiceAdmin : IGenerateUpdateDelete<RefundRequest>
    {
        public bool ApproveRefund(Guid refundId);
        public bool RejectRefund(Guid refundId);
        public IEnumerable<RefundRequest> GetAllPendingApproval();
        public IEnumerable<RefundRequest> GetAllRequiringPayment();
    }
}
