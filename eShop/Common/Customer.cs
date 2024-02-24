using Common.Base;

namespace Common
{
    public class Customer : Person
    {
        public override S Visit<S>(Func<Person, S> visitor)
        {
            return visitor(this);
        }
    }
}
