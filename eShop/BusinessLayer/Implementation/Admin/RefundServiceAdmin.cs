using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin
{
    public class RefundServiceAdmin(IRefundDataAccess dataAccess) : IRefundServiceAdmin
    {
        private readonly IRefundDataAccess _dataAccess = dataAccess;

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

        public bool Delete(Refund refund)
        {
            _dataAccess.Delete(refund);
            return true;
        }

        public bool Generate(Refund refund)
        {
            _dataAccess.Generate(refund);
            return true;
        }

        public bool RejectRefund(Guid refundId)
        {
            var refund = new Refund()
            {
                AltId = refundId
            };
            return _dataAccess.Delete(refund);
        }

        public bool Update(Refund refund)
        {
            return _dataAccess.Update(refund);
        }

        public IEnumerable<Refund> GetAllOutstandingRefunds()
        {
            return this._dataAccess.GetAllRequiringApproval();
        }
    }
}
