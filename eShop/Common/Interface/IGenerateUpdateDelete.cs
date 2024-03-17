namespace Common.Interface
{
    public interface IGenerateUpdateDelete<T>
    {
        public bool Generate(T t);
        public bool Update(T t);
        public bool LogicalDelete(T t);
    }
}
