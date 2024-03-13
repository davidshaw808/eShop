namespace Common.Interface
{
    public interface IElement<T>
    {
        public int? Id { get; set; }
        public bool Active { get; set; }

        public T Visit(IVisitor<T> visitor);
    }
}
