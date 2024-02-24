namespace Common.Interface
{
    public interface IElement<T>
    {
        public int? Id { get; set; }
        public bool Active { get; set; }

        public S Visit<S>(Func<T, S> visitor);
    }
}
