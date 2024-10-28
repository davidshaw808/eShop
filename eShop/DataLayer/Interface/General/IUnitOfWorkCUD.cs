namespace DataLayer.Interface.General;

public interface IUnitOfWorkCUD<T>
{
    void GenerateElementInUoW(T t);
    void UpdateElementInUoW(T t);
    void LogicalDeleteElementInUow(T t);
}