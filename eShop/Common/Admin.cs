using Common.Base;

namespace Common
{
    public class Admin : Person
    {
        public override S Visit<S>(Func<Person, S> visitor)
        {
            return visitor(this);
        }
    }
}
