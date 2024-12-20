using Common.Interface;

namespace DataLayer.Interface.General;

/// <summary>
/// interface for tracked data base interactions - changes should persist after being called
/// </summary>
/// <typeparam name="T">the domain object to perform the operation on</typeparam>
public interface IUnitOfWorkCUD<T>
{
    void GenerateElementInUoW(T t);
    void UpdateElementInUoW(T t);
    void LogicalDeleteElementInUow(T t);
}