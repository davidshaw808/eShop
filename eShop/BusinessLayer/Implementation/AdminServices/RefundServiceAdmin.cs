using BusinessLayer.Interface.Admin;
using Common.Models.Immutable;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin;

public class RefundServiceAdmin(IFinancialTransactionDataAccess dataAccess) : IRefundServiceAdmin
{
    private readonly IFinancialTransactionDataAccess _dataAccess = dataAccess;

    public bool ApproveRefund(Guid refundId)
    {
        var refund = _dataAccess.Get(refundId);
        if (refund == null)
        {
            return false;
        }
        refund.DateApproved = DateTime.UtcNow;
        _dataAccess.Update(refund);
        return true;
    }

    public bool LogicalDelete(FinancialTransaction refund)
    {
        _dataAccess.LogicalDelete(refund);
        return true;
    }

    public bool Generate(FinancialTransaction refund)
    {
        _dataAccess.Generate(refund);
        return true;
    }

    public bool RejectRefund(Guid refundId)
    {
        var refund = new FinancialTransaction()
        {
            Key = refundId
        };
        return _dataAccess.LogicalDelete(refund);
    }

    public bool Update(FinancialTransaction refund)
    {
        return _dataAccess.Update(refund);
    }

    public IEnumerable<FinancialTransaction> GetAllPendingApproval()
    {
        return this._dataAccess.GetAll((FinancialTransaction r) => r.DateApproved == null);
    }

    public IEnumerable<FinancialTransaction> GetAllRequiringPayment()
    {
        return this._dataAccess.GetAll((FinancialTransaction r) => r.DateApproved != null && r.DatePaid == null);
    }
}
