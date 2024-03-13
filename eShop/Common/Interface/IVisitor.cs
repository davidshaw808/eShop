namespace Common.Interface
{
    public interface IVisitor<T>
    {
        public T Visit(IElement<T> visitor);
    }
}
