namespace Common.Interface;

public interface IParentChild<T>
{
    public T? Parent { get; set; }
    public ICollection<T>? Children { get; set; }
}
