namespace DataLayer.Interface.General;

/// <summary>
/// interface for atomic data base interactions - no changes sholud persist after being called
/// </summary>
/// <typeparam name="T">the domain object to perform the operation on</typeparam>
public interface IAtomicCRUD<T>
{
    Task GenerateAtomicAsync(T t);
    Task<T?> GetAtomicAsync(Guid key);
    Task<bool> UpdateAtomicAsync(T t);
    Task<bool> LogicalDeleteAtomicAsync(T t);
}
