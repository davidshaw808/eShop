using Common.Base;
using Common.Interface;

namespace Common;

public class AdminUser : Person, IElement<AdminUser>
{
    public U Visit<U>(IVisitor<AdminUser,U> visitor)
    {
        return visitor.Visit(this);
    }
}
