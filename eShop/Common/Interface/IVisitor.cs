namespace Common.Interface;

public interface IVisitor<T,U>
{
    public U Visit(T visitor);
}
