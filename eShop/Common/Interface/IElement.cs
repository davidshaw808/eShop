namespace Common.Interface
{
    public interface IInternalElement<T> : IElement<T>
    {
        public int? Id { get; set; }
    }

    public interface IElement<T>
    {
        public bool Active { get; set; }
        public T Visit(IVisitor<T> visitor);
    }
}
