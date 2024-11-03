namespace DataLayer.Interface.General;

/// <summary>
/// interface for atomic data base interactions - no changes sholud persist after being called
/// </summary>
/// <typeparam name="T">the domain object to perform the operation on</typeparam>
public interface IAtomicCRUD<T>
{
    Task GenerateAtomicAsync(T t);
    Task<T?> ReadAtomicAsync(Guid key);
    Task<int> UpdateAtomicAsync(T t);
    Task<int> LogicalDeleteAtomicAsync(T t);
}
