using Common;
using Common.Interface;

namespace BusinessLayer.CommonVisitor
{
    public class BeforeSerialise: IVisitor<Address>, IVisitor<Order>, IVisitor<RefundRequest>
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

        public RefundRequest Visit(IElement<RefundRequest> visitor)
        {
            var ret = (RefundRequest)visitor;
            if (ret.AltId == null)
            {
                throw new ArgumentException("AltId cannot be null when serialising a Refund");
            }
            ret.Id = null;
            return ret;
        }

        public Product Visit(IElement<Product> visitor)
        {
            var ret = (Product)visitor;
            if (ret.AltId == null)
            {
                throw new ArgumentException("AltId cannot be null when serialising a Product");
            }
            ret.Id = null;
            return ret;
        }
    }
}
