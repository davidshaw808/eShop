using Common.Base;
using Common.Interface;

namespace Common
{
    public class Customer : Person
    {
        public override Person Visit(IVisitor<Person> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
