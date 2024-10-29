namespace Common.Interface;

public interface IParentChild<T>
{
    public T? Parent { get; set; }
    public IList<T>? Children { get; set; }
}
