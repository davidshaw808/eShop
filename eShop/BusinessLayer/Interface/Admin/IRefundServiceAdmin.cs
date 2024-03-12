using Common.Interface;
using Common;

namespace BusinessLayer.Interface.Admin
{
    public interface IRefundServiceAdmin : IGenerateUpdateDelete<Refund>
    {
        public bool ApproveRefund(Guid refundId);
        public bool RejectRefund(Guid refundId);
        public IEnumerable<Refund> GetAllOutstandingRefunds();
    }
}
