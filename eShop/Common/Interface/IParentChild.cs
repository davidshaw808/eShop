namespace Common.Interface;

public interface IParentChild
{
    public IParentChild? Parent { get; set; }
    public IList<IParentChild>? Children { get; set; }
}
