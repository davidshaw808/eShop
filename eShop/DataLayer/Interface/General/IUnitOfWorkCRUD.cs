using Common.Interface;

namespace DataLayer.Interface.General;

/// <summary>
/// interface for untrcaked atomic one-off data base interactions - changes are implemented immediately
/// </summary>
/// <typeparam name="T">the domain object to perform the operation on</typeparam>
public interface IUnitOfWorkCRUD<T>
{
    void GenerateElementInUoW(T t);
    Task UpdateElementInUoW(T t);
    Task<T?> GetElementInUoW(Guid t);
    Task LogicalDeleteElementInUow(T t);
}