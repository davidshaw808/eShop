using Common.Base;
using Common.Interface;

namespace Common
{
    public class Customer : Person, IElement<Customer>
    {
        public DateTime? RemoveAllCustomerDataRequest { get; set; }

        public Customer Visit(IVisitor<Customer> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
