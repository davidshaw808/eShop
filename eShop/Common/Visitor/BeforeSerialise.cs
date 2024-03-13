using Common.Interface;
namespace Common.Visitor
{
    public class BeforeSerialise : IVisitor<Address>, IVisitor<Order>, IVisitor<Refund>
    {
        public Address Visit(IElement<Address> visitor)
        {
            var ret = (Address)visitor;
            if(ret.AltId == null)
            {
                throw new ArgumentException("AltId cannot be null when serialising an Address");
            }
            ret.Id = null;
            return ret ;
        }

        public Order Visit(IElement<Order> visitor)
        {
            var ret = (Order)visitor;
            if (ret.AltId == null)
            {
                throw new ArgumentException("AltId cannot be null when serialising an Order");
            }
            ret.Id = null;
            return ret;
        }

        public Refund Visit(IElement<Refund> visitor)
        {
            var ret = (Refund)visitor;
            if (ret.AltId == null)
            {
                throw new ArgumentException("AltId cannot be null when serialising a Refund");
            }
            ret.Id = null;
            return ret;
        }
    }
}
