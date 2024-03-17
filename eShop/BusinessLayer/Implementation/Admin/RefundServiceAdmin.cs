using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin
{
    public class RefundServiceAdmin(IFinancialTransaction dataAccess) : IRefundServiceAdmin
    {
        private readonly IFinancialTransaction _dataAccess = dataAccess;

        public bool ApproveRefund(Guid refundId)
        {
            var refund = _dataAccess.Get(refundId);
            if (refund == null)
            {
                return false;
            }
            refund.DateApproved = DateTime.Now;
            _dataAccess.Update(refund);
            return true;
        }

        public bool LogicalDelete(RefundRequest refund)
        {
            _dataAccess.LogicalDelete(refund);
            return true;
        }

        public bool Generate(RefundRequest refund)
        {
            _dataAccess.Generate(refund);
            return true;
        }

        public bool RejectRefund(Guid refundId)
        {
            var refund = new RefundRequest()
            {
                AltId = refundId
            };
            return _dataAccess.LogicalDelete(refund);
        }

        public bool Update(RefundRequest refund)
        {
            return _dataAccess.Update(refund);
        }

        public IEnumerable<RefundRequest> GetAllPendingApproval()
        {
            return this._dataAccess.GetAll((RefundRequest r) => r.DateApproved == null);
        }

        public IEnumerable<RefundRequest> GetAllRequiringPayment()
        {
            return this._dataAccess.GetAll((RefundRequest r) => r.DateApproved != null && r.DatePaid == null);
        }
    }
}
