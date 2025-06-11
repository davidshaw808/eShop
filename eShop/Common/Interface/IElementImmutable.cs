namespace Common.Interface;

public interface IElementImmutable
{
    int Id { get; init; }
}
public interface IElementKeyImmutable
{
    Guid Key { get; init; }
}

public interface IElementImmutable<T> : IElementKeyImmutable, IElementImmutable, IVisit<T>
{
    bool Active { get; set; }
}
