using Common.Base;
using Common.Interface;

namespace Common
{
    public class Admin : Person, IElement<Admin>
    {
        public Admin Visit(IVisitor<Admin> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
