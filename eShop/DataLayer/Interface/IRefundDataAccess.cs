using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IRefundDataAccess : IGenerateUpdateDelete<Refund>
    {
        public Refund? Get(Guid id);
        public IEnumerable<Refund> GetAllRequiringApproval();
    }
}
