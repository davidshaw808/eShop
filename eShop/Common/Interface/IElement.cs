namespace Common.Interface;

public interface IElementId
{
    int? Id { get; set; }
}
public interface IElementKey
{
    Guid? Key { get; set; }
}

public interface IVisit<T>
{ 
    U Visit<U>(IVisitor<T,U> visitor);
}

public interface IElement<T> : IElementKey, IElementId, IVisit<T>
{
    bool Active { get; set; }
}
